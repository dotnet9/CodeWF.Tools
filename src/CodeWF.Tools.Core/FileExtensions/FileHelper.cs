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

    public static void OpenFolderAndSelectFile(string fileFullName)
    {
        var path = fileFullName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = fileFullName.Replace("/", "\\");
        }

        var psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
        {
            Arguments = $"/e,/select,\"{path}\""
        };
        System.Diagnostics.Process.Start(psi);
    }

    public static void OpenFolder(string folderFullName)
    {
        var path = folderFullName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = folderFullName.Replace("/", "\\");
        }

        var psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
        {
            Arguments = path
        };
        System.Diagnostics.Process.Start(psi);
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
}