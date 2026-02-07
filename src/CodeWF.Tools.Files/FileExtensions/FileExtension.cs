using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeWF.Tools.Files.FileExtensions;

public static class FileExtension
{
    public static bool IsPathInUpperTwoLevelsOfCurrentDir(string targetPath)
    {
        string currentDir = Path.GetFullPath(Directory.GetCurrentDirectory());
        currentDir = AddTrailingDirectorySeparator(currentDir);
        string upperTwoDir = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
        upperTwoDir = AddTrailingDirectorySeparator(upperTwoDir);
        string normalizedTarget = Path.GetFullPath(targetPath);
        if (File.Exists(normalizedTarget))
        {
            normalizedTarget = Path.GetDirectoryName(normalizedTarget)!;
        }
        normalizedTarget = AddTrailingDirectorySeparator(normalizedTarget);


        return normalizedTarget.StartsWith(upperTwoDir, StringComparison.Ordinal);
    }

    private static string AddTrailingDirectorySeparator(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;
        if (!Path.EndsInDirectorySeparator(path))
        {
            path += Path.DirectorySeparatorChar;
        }
        return path;
    }
}