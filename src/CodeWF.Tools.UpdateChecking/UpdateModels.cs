namespace CodeWF.Tools.UpdateChecking;

/// <summary>检查结果，区分无更新和检查失败。</summary>
public sealed record UpdateCheckResult(UpdateInfo? Update, bool Succeeded, string? Error)
{
    public static UpdateCheckResult Latest() => new(null, true, null);

    public static UpdateCheckResult Failed(string error) => new(null, false, error);
}

/// <summary>发现新版本时的信息。</summary>
public sealed record UpdateInfo(
    Version Version,
    string Tag,
    string Title,
    string? Notes,
    string PageUrl,
    string? AssetUrl,
    string? AssetName,
    string? ChecksumUrl = null);
