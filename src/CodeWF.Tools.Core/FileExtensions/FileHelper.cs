using System.IO;
using System.Runtime.InteropServices;

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
}