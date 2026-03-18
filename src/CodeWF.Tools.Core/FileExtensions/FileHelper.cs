using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeWF.Tools.FileExtensions;

public static class FileHelper
{
    public static string GetTempFileName(string cacheDirName)
    {
        var cacheDirFullPath = Path.Combine(Path.GetTempPath(), cacheDirName, "Cache");
        if (!Directory.Exists(cacheDirFullPath))
        {
            Directory.CreateDirectory(cacheDirFullPath);
        }

        var fn = Path.Combine(cacheDirFullPath, Path.GetRandomFileName());
        return fn;
    }

    public static void DeleteFileIfExist(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// 跨平台打开文件夹并选中指定文件
    /// </summary>
    /// <param name="fileFullName">文件的完整路径</param>
    /// <exception cref="FileNotFoundException">文件不存在时抛出</exception>
    public static void OpenFolderAndSelectFile(string fileFullName)
    {
        // 校验文件是否存在
        if (!File.Exists(fileFullName))
        {
            throw new FileNotFoundException("The specified file does not exist", fileFullName);
        }

        string normalizedPath = NormalizePathSeparators(fileFullName);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows 逻辑：调用Explorer并选中文件
            var psi = new ProcessStartInfo("Explorer.exe")
            {
                Arguments = $"/e,/select,\"{normalizedPath}\""
            };
            Process.Start(psi);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux 逻辑：根据桌面环境调用对应文件管理器
            OpenLinuxFolderAndSelectFile(normalizedPath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS 逻辑：使用open -R选中文件
            var psi = new ProcessStartInfo("open")
            {
                Arguments = $"-R \"{normalizedPath}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        else
        {
            // 未知系统：兜底打开文件所在目录
            OpenFolder(Path.GetDirectoryName(normalizedPath));
        }
    }

    /// <summary>
    /// 跨平台打开指定文件夹
    /// </summary>
    /// <param name="folderFullName">文件夹的完整路径</param>
    /// <exception cref="DirectoryNotFoundException">文件夹不存在时抛出</exception>
    public static void OpenFolder(string folderFullName)
    {
        // 校验文件夹是否存在
        if (!Directory.Exists(folderFullName))
        {
            throw new DirectoryNotFoundException("The specified folder does not exist：" + folderFullName);
        }

        string normalizedPath = NormalizePathSeparators(folderFullName);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows 逻辑：调用Explorer打开文件夹
            var psi = new ProcessStartInfo("Explorer.exe")
            {
                Arguments = normalizedPath
            };
            Process.Start(psi);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux 逻辑：通用xdg-open打开文件夹
            var psi = new ProcessStartInfo("xdg-open")
            {
                Arguments = $"\"{normalizedPath}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS 逻辑：使用open打开文件夹
            var psi = new ProcessStartInfo("open")
            {
                Arguments = $"\"{normalizedPath}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        else
        {
            throw new PlatformNotSupportedException("The current operating system does not support this operation");
        }
    }

    public static void CreateFolderIfNotExist(string folderFullPath)
    {
        if (Directory.Exists(folderFullPath)) return;

        Directory.CreateDirectory(folderFullPath);
    }

    public static Encoding GetFileEncodeType(string filename)
    {
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        // 读取文件前几个字节以检查 BOM
        var bom = new byte[4];
        fs.ReadExactly(bom, 0, bom.Length);
        fs.Position = 0;

        switch (bom[0])
        {
            // 检查 UTF-8 BOM
            case 0xEF when bom[1] == 0xBB && bom[2] == 0xBF:
                return Encoding.UTF8;
            // 检查 UTF-16 Big Endian BOM
            case 0xFE when bom[1] == 0xFF:
                return Encoding.BigEndianUnicode;
            // 检查 UTF-16 Little Endian BOM
            case 0xFF when bom[1] == 0xFE:
                return Encoding.Unicode;
            // 检查 UTF-32 Big Endian BOM
            case 0x00 when bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF:
                return Encoding.UTF32;
            // 检查 UTF-32 Little Endian BOM
            case 0xFF when bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00:
                return Encoding.GetEncoding("UTF-32BE");
        }

        // 没有 BOM，尝试不同编码读取
        Encoding[] encodings =
        [
            Encoding.UTF8,
            Encoding.GetEncoding("GB2312"),
            Encoding.Default
        ];

        const int bufferSize = 8192; // 每次读取的缓冲区大小
        var buffer = new byte[bufferSize];

        foreach (var encoding in encodings)
        {
            fs.Position = 0; // 重置文件流位置
            var isEncodingCorrect = true;

            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, bufferSize)) > 0)
            {
                try
                {
                    // 使用当前编码将读取的字节内容解码为字符串
                    var decodedString = encoding.GetString(buffer, 0, bytesRead);
                    // 再将解码后的字符串使用相同编码重新编码为字节数组
                    var reEncodedBytes = encoding.GetBytes(decodedString);

                    // 比较重新编码后的字节数组与原始读取的字节内容是否一致
                    for (var i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == reEncodedBytes[i]) continue;
                        isEncodingCorrect = false;
                        break;
                    }

                    if (!isEncodingCorrect)
                    {
                        break;
                    }
                }
                catch (DecoderFallbackException)
                {
                    isEncodingCorrect = false;
                    break;
                }
            }

            if (isEncodingCorrect)
            {
                return encoding;
            }
        }

        // 都失败了，返回默认编码
        return Encoding.Default;
    }

    /// <summary>
    /// 纯手工替换路径分隔符为当前系统标准分隔符（不校验路径是否存在、不解析绝对路径）
    /// </summary>
    /// <param name="inputPath">原始路径（可混写/、\）</param>
    /// <returns>标准化分隔符的路径（保持相对/绝对路径格式）</returns>
    /// <exception cref="ArgumentNullException">输入为空时抛出</exception>
    public static string NormalizePathSeparators(string inputPath)
    {
        // 基础校验：仅判空，不做其他校验
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            throw new ArgumentNullException(nameof(inputPath), "输入路径不能为空或仅包含空白字符");
        }

        // 1. 获取当前系统的标准分隔符
        var targetSeparator = Path.DirectorySeparatorChar;
        // 2. 定义需要替换的「非标准分隔符」
        var sourceSeparator = targetSeparator == '\\' ? '/' : '\\';

        // 3. 纯字符串替换：将所有非标准分隔符换成系统标准分隔符
        var normalizedPath = inputPath.Replace(sourceSeparator, targetSeparator);

        // 可选：清理连续的重复分隔符（比如 "home//user" → "home/user"）
        normalizedPath = CleanDuplicateSeparators(normalizedPath, targetSeparator);

        return normalizedPath;
    }



    #region 私有辅助方法
    
    /// <summary>
    /// Linux下打开文件夹并选中文件（适配主流桌面环境）
    /// </summary>
    /// <param name="filePath">文件完整路径</param>
    private static void OpenLinuxFolderAndSelectFile(string filePath)
    {
        var desktopEnv = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.ToLower() ?? "";
        var (fileManager, args) = GetLinuxFileManagerArgs(desktopEnv, filePath);

        try
        {
            var psi = new ProcessStartInfo(fileManager, args)
            {
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        catch (Exception)
        {
            // 兜底：仅打开文件所在目录
            OpenFolder(Path.GetDirectoryName(filePath));
        }
    }

    /// <summary>
    /// 根据Linux桌面环境获取文件管理器及参数
    /// </summary>
    /// <param name="desktopEnv">桌面环境标识</param>
    /// <param name="filePath">文件完整路径</param>
    /// <returns>（文件管理器命令，参数）</returns>
    private static (string, string) GetLinuxFileManagerArgs(string desktopEnv, string filePath)
    {
        if (desktopEnv.Contains("gnome") || desktopEnv.Contains("unity"))
        {
            // GNOME/Unity - Nautilus
            return ("nautilus", $"--select \"{filePath}\"");
        }

        if (desktopEnv.Contains("kde"))
        {
            // KDE - Dolphin
            return ("dolphin", $"--select \"{filePath}\"");
        }

        if (desktopEnv.Contains("xfce"))
        {
            // XFCE - Thunar
            return ("thunar", $"--select \"{filePath}\"");
        }

        if (desktopEnv.Contains("mate"))
        {
            // MATE - Caja
            return ("caja", $"--select \"{filePath}\"");
        }

        if (desktopEnv.Contains("lxqt"))
        {
            // LXQT - PCManFM-Qt
            return ("pcmanfm-qt", $"--select \"{filePath}\"");
        }

        // 未知桌面环境：仅打开目录
        return ("xdg-open", $"\"{Path.GetDirectoryName(filePath)}\"");
    }

    /// <summary>
    /// 辅助方法：清理路径中连续的重复分隔符
    /// </summary>
    private static string CleanDuplicateSeparators(string path, char separator)
    {
        if (string.IsNullOrEmpty(path)) return path;

        // 用 StringBuilder 高效处理字符串拼接
        var sb = new System.Text.StringBuilder();
        var lastWasSeparator = false;

        foreach (char c in path)
        {
            if (c == separator)
            {
                if (!lastWasSeparator)
                {
                    sb.Append(c);
                    lastWasSeparator = true;
                }
                // 跳过连续的分隔符
            }
            else
            {
                sb.Append(c);
                lastWasSeparator = false;
            }
        }

        return sb.ToString();
    }

    #endregion
}