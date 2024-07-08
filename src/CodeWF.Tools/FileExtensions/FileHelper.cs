using System.IO;

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
        var psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
        {
            Arguments = "/e,/select," + fileFullName
        };
        System.Diagnostics.Process.Start(psi);
    }

    public static void OpenFolder(string folderFullName)
    {
        var psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
        {
            Arguments = folderFullName
        };
        System.Diagnostics.Process.Start(psi);
    }
}