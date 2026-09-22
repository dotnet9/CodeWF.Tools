using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers.Zip;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Web;
using SharpCompress.Archives.Zip;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace CodeWF.Tools.FileExtensions;

/// <summary>
/// 7z压缩
/// </summary>
public class SevenZipCompressor : ISevenZipCompressor
{
    private readonly HttpClient _httpClient;

    /// <summary>
    ///
    /// </summary>
    /// <param name="httpClient"></param>
    public SevenZipCompressor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    /// <summary>
    /// 解压文件，自动检测压缩包类型
    /// </summary>
    /// <param name="compressedFile">rar文件</param>
    /// <param name="dir">解压到...</param>
    /// <param name="ignoreEmptyDir">忽略空文件夹</param>
    public void Decompress(string compressedFile, string dir, bool ignoreEmptyDir = true)
    {
        DecompressCore(compressedFile, dir, ignoreEmptyDir);
    }

    /// <summary>
    /// 解压文件，自动检测压缩包类型
    /// </summary>
    /// <param name="compressedFile">rar文件</param>
    /// <param name="dir">解压到...</param>
    public void Decompress(string compressedFile, string dir)
    {
        DecompressCore(compressedFile, dir, ignoreEmptyDir: true);
    }

    private static void DecompressCore(string compressedFile, string dir, bool ignoreEmptyDir)
    {
        if (string.IsNullOrEmpty(dir))
        {
            dir = Path.GetDirectoryName(compressedFile) ?? Directory.GetCurrentDirectory();
        }

        var destinationDirectory = Directory.CreateDirectory(dir).FullName;
        using var archive = ArchiveFactory.Open(compressedFile);
        var entries = archive.Entries.ToList();
        var extractionPaths = entries
            .Select(entry => (Entry: entry, Path: GetSafeExtractionPath(destinationDirectory, entry.Key)))
            .ToList();

        foreach (var (entry, path) in extractionPaths)
        {
            if (!string.IsNullOrEmpty(entry.LinkTarget))
            {
                throw new InvalidDataException($"Symbolic link entries are not supported: {entry.Key}");
            }

            if (entry.IsDirectory)
            {
                if (!ignoreEmptyDir)
                {
                    Directory.CreateDirectory(path);
                }

                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            entry.WriteToFile(path, new ExtractionOptions
            {
                ExtractFullPath = false,
                Overwrite = true
            });
        }
    }

    private static string GetSafeExtractionPath(string destinationDirectory, string? entryKey)
    {
        if (string.IsNullOrWhiteSpace(entryKey))
        {
            throw new InvalidDataException("Archive entry path cannot be empty.");
        }

        if (entryKey.Contains('\0'))
        {
            throw new InvalidDataException("Archive entry path cannot contain null characters.");
        }

        var relativePath = entryKey.Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
        if (Path.IsPathRooted(relativePath) || !string.IsNullOrEmpty(Path.GetPathRoot(relativePath)))
        {
            throw new InvalidDataException($"Archive entry path is absolute: {entryKey}");
        }

        var fullPath = Path.GetFullPath(Path.Combine(destinationDirectory, relativePath));
        var relativeToDestination = Path.GetRelativePath(destinationDirectory, fullPath);
        if (relativeToDestination == ".." ||
            relativeToDestination.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal) ||
            Path.IsPathRooted(relativeToDestination))
        {
            throw new InvalidDataException($"Archive entry path escapes the destination directory: {entryKey}");
        }

        return fullPath;
    }

    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="dir">文件夹</param>
    /// <param name="zipFile">压缩到...</param>
    /// <param name="rootdir">压缩包内部根文件夹</param>
    public void Zip(string dir, string zipFile, string rootdir = "")
    {
        Zip(Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories), zipFile, rootdir);
    }

    /// <summary>
    /// 压缩多个文件
    /// </summary>
    /// <param name="files">多个文件路径，文件或文件夹</param>
    /// <param name="zipFile">压缩到...</param>
    /// <param name="rootdir">压缩包内部根文件夹</param>
    public void Zip(IEnumerable<string> files, string zipFile, string rootdir = "")
    {
        using var archive = CreateZipArchive(files, rootdir);
        archive.SaveTo(zipFile, new ZipWriterOptions(CompressionType.LZMA)
        {
            LeaveStreamOpen = true,
            ArchiveEncoding = new ArchiveEncoding()
            {
                Default = Encoding.UTF8
            }
        });
    }

    /// <summary>
    /// 创建zip包
    /// </summary>
    /// <param name="files"></param>
    /// <param name="rootdir"></param>
    /// <param name="archiveType"></param>
    /// <returns></returns>
    private IWritableArchive CreateZipArchive(IEnumerable<string> files, string rootdir)
    {
        var archive = ArchiveFactory.Create(ArchiveType.Zip);
        var dic = GetFileEntryMaps(files);
        var remoteUrls = files.Distinct().Where(s => s.StartsWith("http", StringComparison.OrdinalIgnoreCase)).Select(s =>
        {
            try
            {
                return new Uri(s);
            }
            catch (UriFormatException)
            {
                return null;
            }
        }).OfType<Uri>().ToList();
        foreach (var pair in dic)
        {
            archive.AddEntry(CombineArchiveEntryPath(rootdir, pair.Value), pair.Key);
        }

        if (remoteUrls.Any())
        {
            var streams = DownloadRemoteEntriesAsync(remoteUrls, rootdir).GetAwaiter().GetResult();
            foreach (var pair in streams)
            {
                archive.AddEntry(pair.Key, pair.Value, true);
            }
        }

        return archive;
    }

    private async Task<ConcurrentDictionary<string, Stream>> DownloadRemoteEntriesAsync(IEnumerable<Uri> remoteUrls,
        string rootdir)
    {
        var streams = new ConcurrentDictionary<string, Stream>();
        var tasks = remoteUrls.Select(async url =>
        {
            using var res = await _httpClient.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                return;
            }

            var stream = new MemoryStream();
            await res.Content.CopyToAsync(stream);
            stream.Position = 0;
            var entryName = CombineArchiveEntryPath(rootdir, Path.GetFileName(HttpUtility.UrlDecode(url.AbsolutePath)));
            streams[entryName] = stream;
        });

        await Task.WhenAll(tasks);
        return streams;
    }

    /// <summary>
    /// 获取文件路径和zip-entry的映射
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    private Dictionary<string, string> GetFileEntryMaps(IEnumerable<string> files)
    {
        var fileList = new List<string>();
        foreach (var file in files.Where(s => !s.StartsWith("http", StringComparison.OrdinalIgnoreCase)))
        {
            if (Directory.Exists(file))
            {
                fileList.AddRange(Directory.EnumerateFiles(file, "*", SearchOption.AllDirectories));
            }
            else
            {
                fileList.Add(file);
            }
        }

        if (!fileList.Any())
        {
            return new Dictionary<string, string>();
        }

        var pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var uniqueFiles = fileList
            .Select(Path.GetFullPath)
            .Distinct(pathComparer)
            .ToList();
        var commonDirectory = Path.GetDirectoryName(uniqueFiles[0])!;
        var commonRoot = Path.GetPathRoot(commonDirectory);
        if (uniqueFiles.Any(file => !string.Equals(Path.GetPathRoot(file), commonRoot,
                OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Files on different volume roots cannot be added to one archive.");
        }

        foreach (var file in uniqueFiles.Skip(1))
        {
            var fileDirectory = Path.GetDirectoryName(file)!;
            while (!IsPathWithin(commonDirectory, fileDirectory))
            {
                var parent = Directory.GetParent(commonDirectory)?.FullName;
                if (parent is null)
                {
                    break;
                }

                commonDirectory = parent;
            }
        }

        return uniqueFiles.ToDictionary(
            file => file,
            file => Path.GetRelativePath(commonDirectory, file),
            pathComparer);
    }

    private static bool IsPathWithin(string rootDirectory, string path)
    {
        var relativePath = Path.GetRelativePath(rootDirectory, path);
        return relativePath == "." ||
               (!Path.IsPathRooted(relativePath) &&
                relativePath != ".." &&
                !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
    }

    private static string CombineArchiveEntryPath(string rootdir, string entryPath)
    {
        var normalizedRoot = rootdir.Trim('/', '\\');
        var normalizedEntry = entryPath.Replace('\\', '/');
        return string.IsNullOrEmpty(normalizedRoot)
            ? normalizedEntry
            : $"{normalizedRoot}/{normalizedEntry}";
    }
}
