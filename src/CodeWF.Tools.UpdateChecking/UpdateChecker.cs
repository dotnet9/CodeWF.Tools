using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CodeWF.Tools.UpdateChecking;

/// <summary>检查 GitHub Releases 更新。</summary>
public interface IUpdateChecker
{
    Task<UpdateCheckResult> CheckAsync(Version current, CancellationToken cancellationToken = default);
}

/// <summary>更新检查缓存：ETag 配 API 条件请求；release 内容配网页检查——tag 没变时直接复用做版本比较。</summary>
public sealed record UpdateCheckState(string? Etag, GitHubRelease? Release);

/// <summary>
/// 检查 GitHub Releases 更新，默认路径完全不碰 api.github.com 的每小时配额：
/// - releases/latest 的 302 落点就是最新发布 tag（未认证 API 的配额只有 60 次/小时/IP，
///   走代理时被同出口用户共享、极易耗尽，网页端点没有这个限制）；
/// - expanded_assets/{tag} 是发布页懒加载资产列表的接口，从中解析资产名与下载直链；
/// - tag 与上次缓存一致时连资产请求都省掉，用缓存的 release 做版本比较。
/// 网页端点拿不到（改版/超时/网络）才退回 Releases API：ETag 条件请求 304 不计配额，
/// 403/429 按 Retry-After / X-RateLimit-Reset 退避，恢复前不再发请求。
/// 任何网络/解析异常都吞掉返回失败结果，不打扰用户。
/// </summary>
public sealed class UpdateChecker : IUpdateChecker
{
    private const string DefaultApiBase = "https://api.github.com";
    private const string DefaultWebBase = "https://github.com";

    private readonly HttpClient _http;
    private readonly string _owner;
    private readonly string _repo;
    private readonly string _apiBase;
    private readonly string _webBase;
    private readonly Action<string>? _log;
    private readonly bool _preferInstaller;
    private readonly string? _runtimeIdentifier;
    private readonly string? _stateFile;
    private DateTime _blockedUntilUtc = DateTime.MinValue;

    /// <summary>便利构造：内部创建 HttpClient（web 端点优先 + API 回退的默认配置）。</summary>
    public UpdateChecker(string owner, string repo, Action<string>? log = null)
        : this(new HttpClient { Timeout = TimeSpan.FromSeconds(12) }, owner, repo, log: log)
    {
    }

    public UpdateChecker(
        HttpClient http,
        string owner,
        string repo,
        string apiBase = DefaultApiBase,
        Action<string>? log = null,
        bool preferInstaller = true,
        string? runtimeIdentifier = null,
        string? stateFile = null,
        string? webBase = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _owner = owner;
        _repo = repo;
        _apiBase = apiBase.TrimEnd('/');
        _webBase = (webBase ?? DefaultWebBase).TrimEnd('/');
        _log = log;
        _preferInstaller = preferInstaller;
        _runtimeIdentifier = runtimeIdentifier;
        _stateFile = stateFile;
    }

    public async Task<UpdateCheckResult> CheckAsync(Version current, CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateCheckResult? viaWeb = await CheckViaWebAsync(current, cancellationToken).ConfigureAwait(false);
            if (viaWeb is not null)
            {
                return viaWeb;
            }

            return await CheckViaApiAsync(current, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return UpdateCheckResult.Failed("操作已取消");
        }
        catch (Exception ex)
        {
            _log?.Invoke($"检查更新异常：{ex.Message}");
            return UpdateCheckResult.Failed(ex.Message);
        }
    }

    /// <summary>网页端点检查：不占 API 配额。拿不到结果返回 null，由调用方退回 API。</summary>
    private async Task<UpdateCheckResult?> CheckViaWebAsync(Version current, CancellationToken cancellationToken)
    {
        try
        {
            string? tag = await FetchLatestTagViaRedirectAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }

            UpdateCheckState? state = TryReadState();
            if (state?.Release is { } cached
                && string.Equals(cached.TagName, tag, StringComparison.OrdinalIgnoreCase))
            {
                // 没发新版：缓存的 release 直接做版本比较，资产请求也省掉
                return EvaluateRelease(cached, current);
            }

            GitHubRelease? release = await FetchWebReleaseAsync(tag, cancellationToken).ConfigureAwait(false);
            if (release is null)
            {
                return null;
            }

            TrySaveState(new UpdateCheckState(Etag: null, release));
            return EvaluateRelease(release, current);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // HttpClient 超时也抛 TaskCanceledException：网页路径超时不算失败，退回 API
            _log?.Invoke("网页检查超时，退回 API");
            return null;
        }
        catch (Exception ex)
        {
            _log?.Invoke($"网页检查失败，退回 API：{ex.Message}");
            return null;
        }
    }

    /// <summary>releases/latest 的 302 落点就是 /releases/tag/{tag}；只取响应头，不下载页面。</summary>
    private async Task<string?> FetchLatestTagViaRedirectAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_webBase}/{_owner}/{_repo}/releases/latest");
        request.Headers.TryAddWithoutValidation("User-Agent", "QuickApp-UpdateChecker");
        using HttpResponseMessage response = await _http
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        // HttpClient 自动跟随重定向时最终地址写在 RequestMessage 上；跟随被禁用时读 Location 头
        Uri? finalUri = IsRedirect(response.StatusCode)
            ? response.Headers.Location
            : response.RequestMessage?.RequestUri;
        string? tag = ExtractTag(finalUri);
        if (tag is null)
        {
            _log?.Invoke($"网页检查未解析到最新 tag（HTTP {(int)response.StatusCode}）");
        }

        return tag;
    }

    /// <summary>expanded_assets 是发布页懒加载资产列表的接口；从中拼出 release 模型（说明等字段拿不到，界面未用到）。</summary>
    private async Task<GitHubRelease?> FetchWebReleaseAsync(string tag, CancellationToken cancellationToken)
    {
        string url = $"{_webBase}/{_owner}/{_repo}/releases/expanded_assets/{tag}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("User-Agent", "QuickApp-UpdateChecker");
        using HttpResponseMessage response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            _log?.Invoke($"资产列表拉取失败：HTTP {(int)response.StatusCode}");
            return null;
        }

        string html = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        List<GitHubAsset> assets = ParseDownloadAssets(html, tag, _webBase);
        return new GitHubRelease
        {
            TagName = tag,
            HtmlUrl = $"{_webBase}/{_owner}/{_repo}/releases/tag/{tag}",
            Assets = assets.ToArray()
        };
    }

    /// <summary>回退：Releases API。ETag 条件请求 304 不计配额；403/429 按指示退避。</summary>
    private async Task<UpdateCheckResult> CheckViaApiAsync(Version current, CancellationToken cancellationToken)
    {
        if (DateTime.UtcNow < _blockedUntilUtc)
        {
            return UpdateCheckResult.Failed(RateLimitMessage);
        }

        string url = $"{_apiBase}/repos/{_owner}/{_repo}/releases/latest";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("Accept", "application/vnd.github+json");
        request.Headers.TryAddWithoutValidation("User-Agent", "QuickApp-UpdateChecker");
        UpdateCheckState? state = TryReadState();
        if (!string.IsNullOrEmpty(state?.Etag))
        {
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(state.Etag));
        }

        using HttpResponseMessage response = await _http.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            // 最新发布没变：条件请求不计配额，用缓存的 release 内容做版本比较
            return state?.Release is { } cached
                ? EvaluateRelease(cached, current)
                : UpdateCheckResult.Latest();
        }

        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
        {
            _blockedUntilUtc = ParseBlockedUntil(response);
            _log?.Invoke($"检查更新被限流（HTTP {(int)response.StatusCode}），恢复前不再请求 API");
            return UpdateCheckResult.Failed(RateLimitMessage);
        }

        if (!response.IsSuccessStatusCode)
        {
            _log?.Invoke($"检查更新失败：HTTP {(int)response.StatusCode}");
            return UpdateCheckResult.Failed($"HTTP {(int)response.StatusCode}");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        GitHubRelease? release = JsonSerializer.Deserialize<GitHubRelease>(json, GitHubJsonOptions.DefaultOptions);
        if (release is null || release.Draft || release.Prerelease)
        {
            return UpdateCheckResult.Latest();
        }

        TrySaveState(new UpdateCheckState(response.Headers.ETag?.Tag, release));
        return EvaluateRelease(release, current);
    }

    private static bool IsRedirect(HttpStatusCode statusCode)
        => statusCode is HttpStatusCode.Moved
            or HttpStatusCode.Found
            or HttpStatusCode.SeeOther
            or HttpStatusCode.TemporaryRedirect
            or HttpStatusCode.PermanentRedirect;

    /// <summary>从 …/releases/tag/{tag} 的落点地址抠出 tag；跟随与未跟随两种响应都要顾及。</summary>
    private static string? ExtractTag(Uri? uri)
    {
        if (uri is null)
        {
            return null;
        }

        const string marker = "/releases/tag/";
        string path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.OriginalString;
        int start = path.IndexOf(marker, StringComparison.Ordinal);
        if (start < 0)
        {
            return null;
        }

        string tag = Uri.UnescapeDataString(path[(start + marker.Length)..]).TrimEnd('/');
        return tag.Length > 0 ? tag : null;
    }

    /// <summary>从 expanded_assets 的 HTML 里抠出 /releases/download/{tag}/{文件名} 资产；
    /// 简单字符串扫描而不引 HTML 解析器，保持 AOT 友好。</summary>
    private static List<GitHubAsset> ParseDownloadAssets(string html, string tag, string webBase)
    {
        var assets = new List<GitHubAsset>();
        const string marker = "/releases/download/";
        int index = 0;
        while ((index = html.IndexOf("href=\"", index, StringComparison.Ordinal)) >= 0)
        {
            int start = index + "href=\"".Length;
            int end = html.IndexOf('"', start);
            if (end < 0)
            {
                break;
            }

            index = end + 1;
            string href = html[start..end];
            int pathStart = href.IndexOf(marker, StringComparison.Ordinal);
            if (pathStart < 0)
            {
                continue;
            }

            string remainder = href[(pathStart + marker.Length)..]; // {tag}/{文件名}
            int separator = remainder.IndexOf('/');
            if (separator <= 0)
            {
                continue;
            }

            string hrefTag = Uri.UnescapeDataString(remainder[..separator]);
            string name = Uri.UnescapeDataString(remainder[(separator + 1)..]);
            if (name.Length == 0 || !string.Equals(hrefTag, tag, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // 同一资产在页面里可能以根相对与绝对两种形式各出现一次
            if (assets.Any(a => string.Equals(a.Name, name, StringComparison.Ordinal)))
            {
                continue;
            }

            bool absolute = href.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || href.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
            assets.Add(new GitHubAsset
            {
                Name = name,
                BrowserDownloadUrl = absolute ? href : webBase + href
            });
        }

        return assets;
    }

    private UpdateCheckResult EvaluateRelease(GitHubRelease release, Version current)
    {
        Version? candidate = VersionUtil.Parse(release.TagName);
        if (!VersionUtil.IsNewer(candidate, current))
        {
            return UpdateCheckResult.Latest();
        }

        (string? assetUrl, string? assetName, string? checksumUrl) = PickAsset(
            release,
            _runtimeIdentifier ?? CurrentRuntimeIdentifier(),
            _preferInstaller);
        string title = string.IsNullOrWhiteSpace(release.Name) ? release.TagName ?? string.Empty : release.Name!;

        return new UpdateCheckResult(new UpdateInfo(
            candidate!,
            release.TagName ?? string.Empty,
            title,
            release.Body,
            release.HtmlUrl ?? $"https://github.com/{_owner}/{_repo}/releases",
            assetUrl,
            assetName,
            checksumUrl), true, null);
    }

    private string RateLimitMessage
        => "GitHub API 限流中，约 " + _blockedUntilUtc.ToLocalTime().ToString("HH:mm", CultureInfo.CurrentCulture) + " 后恢复";

    /// <summary>从 Retry-After 或 X-RateLimit-Reset 推算限流恢复时间，兜底 10 分钟，最长封顶 1 小时。</summary>
    private static DateTime ParseBlockedUntil(HttpResponseMessage response)
    {
        DateTime now = DateTime.UtcNow;
        TimeSpan? retryAfter = response.Headers.RetryAfter?.Delta;
        if (retryAfter is null && response.Headers.RetryAfter?.Date is { } retryDate)
        {
            retryAfter = retryDate - DateTimeOffset.UtcNow;
        }

        DateTime blocked = retryAfter is { } delay && delay > TimeSpan.Zero
            ? now + delay
            : response.Headers.TryGetValues("X-RateLimit-Reset", out var values)
              && long.TryParse(values.FirstOrDefault(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long resetSeconds)
                ? DateTimeOffset.FromUnixTimeSeconds(resetSeconds).UtcDateTime
                : now + TimeSpan.FromMinutes(10);

        blocked += TimeSpan.FromSeconds(30);
        return blocked > now + TimeSpan.FromHours(1) ? now + TimeSpan.FromHours(1) : blocked;
    }

    private UpdateCheckState? TryReadState()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_stateFile) || !File.Exists(_stateFile))
            {
                return null;
            }

            string json = File.ReadAllText(_stateFile);
            return JsonSerializer.Deserialize<UpdateCheckState>(json, GitHubJsonOptions.DefaultOptions);
        }
        catch
        {
            // 缓存读不出来就当作没有，走普通请求
            return null;
        }
    }

    private void TrySaveState(UpdateCheckState state)
    {
        if (string.IsNullOrWhiteSpace(_stateFile))
        {
            return;
        }

        try
        {
            string? dir = Path.GetDirectoryName(_stateFile);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(_stateFile, JsonSerializer.Serialize(state, GitHubJsonOptions.DefaultOptions));
        }
        catch
        {
            // 缓存写失败不影响本次检查结果
        }
    }

    /// <summary>
    /// 选择当前系统/架构的可下载资产。Windows 使用安装器，Linux 使用 deb，macOS 使用 pkg/dmg；
    /// 对旧版本保留 zip 回退。没有匹配包时交给用户打开 Release 页面。
    /// </summary>
    private static (string? Url, string? Name, string? ChecksumUrl) PickAsset(
        GitHubRelease release,
        string? runtimeIdentifier,
        bool preferInstaller)
    {
        if (release.Assets is null || release.Assets.Length == 0)
        {
            return (null, null, null);
        }

        if (string.IsNullOrWhiteSpace(runtimeIdentifier))
        {
            return (null, null, null);
        }

        string marker = "-" + runtimeIdentifier;
        GitHubAsset? preferred = null;

        // The installer is only published for Windows x64. Keep the zip as a
        // fallback so older releases remain usable during the transition.
        if (preferInstaller && string.Equals(runtimeIdentifier, "win-x64", StringComparison.OrdinalIgnoreCase))
        {
            preferred = release.Assets.FirstOrDefault(a =>
                IsAssetForRuntime(a, marker) &&
                a.Name!.EndsWith("-setup.exe", StringComparison.OrdinalIgnoreCase));
        }

        string nativeExtension = runtimeIdentifier.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)
            ? ".deb"
            : runtimeIdentifier.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)
                ? ".pkg"
                : ".zip";

        preferred ??= release.Assets.FirstOrDefault(a =>
            IsAssetForRuntime(a, marker) &&
            a.Name!.EndsWith(nativeExtension, StringComparison.OrdinalIgnoreCase));

        if (runtimeIdentifier.StartsWith("osx-", StringComparison.OrdinalIgnoreCase))
        {
            preferred ??= release.Assets.FirstOrDefault(a =>
                IsAssetForRuntime(a, marker) &&
                a.Name!.EndsWith(".dmg", StringComparison.OrdinalIgnoreCase));
        }

        // Releases before native installers were introduced only contain ZIP files.
        preferred ??= release.Assets.FirstOrDefault(a =>
            IsAssetForRuntime(a, marker) &&
            a.Name!.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));

        if (preferred is null)
        {
            return (null, null, null);
        }

        GitHubAsset? checksum = release.Assets.FirstOrDefault(asset =>
            string.Equals(asset.Name, preferred.Name + ".sha256", StringComparison.OrdinalIgnoreCase));
        return (preferred.BrowserDownloadUrl, preferred.Name, checksum?.BrowserDownloadUrl);
    }

    private static bool IsAssetForRuntime(GitHubAsset asset, string marker)
    {
        return !string.IsNullOrWhiteSpace(asset.Name) &&
            !string.IsNullOrWhiteSpace(asset.BrowserDownloadUrl) &&
            (asset.Name!.Contains(marker + ".", StringComparison.OrdinalIgnoreCase) ||
             asset.Name.Contains(marker + "-", StringComparison.OrdinalIgnoreCase));
    }

    private static string? CurrentRuntimeIdentifier()
    {
        string os = OperatingSystem.IsWindows()
            ? "win"
            : OperatingSystem.IsLinux()
                ? "linux"
                : OperatingSystem.IsMacOS()
                    ? "osx"
                    : string.Empty;

        string architecture = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "x86",
            Architecture.X64 => "x64",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            _ => string.Empty
        };

        return os.Length == 0 || architecture.Length == 0 ? null : os + "-" + architecture;
    }
}

/// <summary>版本号比较。tag 形如 v1.2.3 / 1.2 / 1.2.3.4 都能吃。</summary>
public static class VersionUtil
{
    /// <summary>把 tag 解析成可比较的 Version，失败返回 null。</summary>
    public static Version? Parse(string? tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        var text = tag.Trim();
        if (text.Length > 0 && (text[0] == 'v' || text[0] == 'V'))
        {
            text = text[1..];
        }

        var dash = text.IndexOfAny(new[] { '-', '+' });
        if (dash > 0)
        {
            text = text[..dash];
        }

        return Version.TryParse(text, out var version) ? Normalize(version) : null;
    }

    /// <summary>candidate 是否比 current 新。两边补齐到四段再比。</summary>
    public static bool IsNewer(Version? candidate, Version? current)
    {
        if (candidate is null)
        {
            return false;
        }

        return current is null || Normalize(candidate) > Normalize(current);
    }

    private static Version Normalize(Version version)
        => new(
            Math.Max(version.Major, 0),
            Math.Max(version.Minor, 0),
            Math.Max(version.Build, 0),
            Math.Max(version.Revision, 0));
}
