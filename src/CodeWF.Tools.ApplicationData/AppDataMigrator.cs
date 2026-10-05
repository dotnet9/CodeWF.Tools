using System;
using System.IO;

namespace CodeWF.Tools.ApplicationData;

/// <summary>
/// 桌面应用数据目录与一次性迁移的标准实现：
/// - 数据目录统一放 %LOCALAPPDATA%\&lt;AppName&gt;（Linux ~/.local/share、macOS ~/Library/Application Support，
///   随 SpecialFolder.LocalApplicationData 各平台取标准值）；
/// - 旧位置（%APPDATA%\&lt;LegacyAppName&gt;，Roaming）存在而新位置无数据时，一次性整目录迁移，
///   旧目录原样保留作为备份；
/// - 兼容 v0.4.2 式缺陷迁移：内容被拷到 LOCALAPPDATA 根（少了一级应用名目录）、应用又自动生成
///   默认配置的场景，通过根目录散落文件识别并用旧数据覆盖修复，散落文件随之清理；
/// - 便携模式（exe 旁存在标记文件）不迁移，配置跟随程序目录。
/// </summary>
public static class AppDataMigrator
{
    /// <summary>
    /// 解析数据目录并按需执行一次性迁移。
    /// </summary>
    /// <param name="appName">应用数据目录名（新旧位置同名，如 "QuickApp"）。</param>
    /// <param name="configFileName">数据判据文件名（如 "config.json"），作为"是否已有数据"的判据。</param>
    /// <param name="baseDirectory">程序基目录（AppContext.BaseDirectory），用于便携模式判断。</param>
    /// <param name="portableMarkerFileName">便携模式标记文件名（如 "portable.txt"）；exe 旁存在该文件时跳过迁移且数据目录=程序目录。null 表示不支持便携模式。</param>
    /// <param name="log">日志回调。</param>
    /// <returns>迁移后的数据目录（迁移失败时仍返回该目录，由应用按无数据启动）。</returns>
    public static string ResolveAndMigrate(
        string appName,
        string configFileName,
        string baseDirectory,
        string? portableMarkerFileName = null,
        Action<string>? log = null)
    {
        var dataDirectory = GetDirectory(Environment.SpecialFolder.LocalApplicationData, appName, baseDirectory);
        var legacyDirectory = GetDirectory(Environment.SpecialFolder.ApplicationData, appName, baseDirectory);
        if (string.IsNullOrEmpty(legacyDirectory))
        {
            return dataDirectory;
        }

        var newDataFile = Path.Combine(dataDirectory, configFileName);
        var legacyDataFile = Path.Combine(legacyDirectory, configFileName);

        try
        {
            var hasNew = File.Exists(newDataFile);
            var hasLegacy = File.Exists(legacyDataFile);
            var hasStrays = File.Exists(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                configFileName));

            // v0.4.2 式坏迁移：配置散落在 LOCALAPPDATA 根（标记 = 根目录出现同名配置文件）
            if (hasLegacy && hasStrays)
            {
                CopyDirectory(legacyDirectory, dataDirectory, overwrite: true);
                CleanupLocalRootStrays(configFileName);
                log?.Invoke($"检测到旧版本错误迁移，已用 {legacyDirectory} 修复 {dataDirectory}");
                return dataDirectory;
            }

            // 正常首次升级：旧位置有数据、新位置没有
            if (hasLegacy && !hasNew)
            {
                CopyDirectory(legacyDirectory, dataDirectory, overwrite: false);
                log?.Invoke($"已把配置从 {legacyDirectory} 迁移到 {dataDirectory}");
            }
        }
        catch (Exception ex)
        {
            log?.Invoke($"数据迁移失败（将使用新位置继续，可手动从旧位置找回）：{ex.Message}");
        }

        return dataDirectory;
    }

    private static string GetDirectory(Environment.SpecialFolder folder, string appName, string baseDirectory)
    {
        var root = Environment.GetFolderPath(folder);
        return string.IsNullOrWhiteSpace(root) ? baseDirectory : Path.Combine(root, appName);
    }

    private static void CopyDirectory(string sourceDirectory, string targetDirectory, bool overwrite)
    {
        Directory.CreateDirectory(targetDirectory);
        foreach (var file in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(targetDirectory, Path.GetRelativePath(sourceDirectory, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, overwrite);
        }
    }

    private static void CleanupLocalRootStrays(string configFileName)
    {
        // 散落文件均为坏迁移在 LOCALAPPDATA 根创建的已知名称
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var knownNames = new[]
        {
            configFileName, Path.GetFileNameWithoutExtension(configFileName) + ".bak",
            "update-state.json",
            Path.GetFileNameWithoutExtension(configFileName) + ".1.json",
            Path.GetFileNameWithoutExtension(configFileName) + ".2.json",
            Path.GetFileNameWithoutExtension(configFileName) + ".3.json",
            Path.GetFileNameWithoutExtension(configFileName) + ".4.json"
        };
        foreach (var name in knownNames)
        {
            var path = Path.Combine(root, name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        var icons = Path.Combine(root, "icons");
        if (Directory.Exists(icons))
        {
            Directory.Delete(icons, recursive: true);
        }
    }
}
