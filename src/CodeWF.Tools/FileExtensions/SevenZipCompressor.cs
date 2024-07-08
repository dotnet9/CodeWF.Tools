using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeWF.Tools.FileExtensions;

/// <summary>
/// 7z压缩
/// </summary>
public class SevenZipCompressor : ISevenZipCompressor
{
    /// <summary>
    /// 解压文件，自动检测压缩包类型
    /// </summary>
    /// <param name="compressedFile">rar文件</param>
    /// <param name="dir">解压到...</param>
    public void Decompress(string compressedFile, string dir)
    {
        if (string.IsNullOrEmpty(dir))
        {
            dir = Path.GetDirectoryName(compressedFile);
        }

        ArchiveFactory.WriteToDirectory(compressedFile, Directory.CreateDirectory(dir).FullName, new ExtractionOptions()
        {
            ExtractFullPath = true,
            Overwrite = true
        });
    }

    /// <summary>
    /// 压缩多个文件
    /// </summary>
    /// <param name="files">多个文件路径，文件或文件夹</param>
    /// <param name="zipFile">压缩到...</param>
    /// <param name="rootdir">压缩包内部根文件夹</param>
    /// <param name="archiveType"></param>
    public void Zip(IEnumerable<string> files, string zipFile, string rootdir = "",
        ArchiveType archiveType = ArchiveType.Zip)
    {
        using var archive = CreateZipArchive(files, rootdir, archiveType);
        archive.SaveTo(zipFile, new WriterOptions(CompressionType.LZMA)
        {
            LeaveStreamOpen = true,
            ArchiveEncoding = new ArchiveEncoding()
            {
                Default = Encoding.UTF8
            }
        });
    }

    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="dir">文件夹</param>
    /// <param name="zipFile">压缩到...</param>
    /// <param name="rootDir">压缩包内部根文件夹</param>
    /// <param name="archiveType"></param>
    public void Zip(string dir, string zipFile, string rootDir = "", ArchiveType archiveType = ArchiveType.Zip)
    {
        if (rootDir == null) throw new ArgumentNullException(nameof(rootDir));
        Zip(Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories), zipFile, rootDir, archiveType);
    }

    /// <summary>
    /// 创建zip包
    /// </summary>
    /// <param name="files"></param>
    /// <param name="rootDir"></param>
    /// <param name="archiveType"></param>
    /// <returns></returns>
    private IWritableArchive CreateZipArchive(IEnumerable<string> files, string rootDir, ArchiveType archiveType)
    {
        var archive = ArchiveFactory.Create(archiveType);
        var dic = GetFileEntryMaps(files);
        foreach (var pair in dic)
        {
            archive.AddEntry(Path.Combine(rootDir, pair.Value), pair.Key);
        }

        return archive;
    }

    /// <summary>
    /// 获取文件路径和zip-entry的映射
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    private Dictionary<string, string> GetFileEntryMaps(IEnumerable<string> files)
    {
        var fileList = new List<string>();

        void GetFilesRecurs(string path)
        {
            //遍历目标文件夹的所有文件
            fileList.AddRange(Directory.GetFiles(path));

            //遍历目标文件夹的所有文件夹
            foreach (var directory in Directory.GetDirectories(path))
            {
                GetFilesRecurs(directory);
            }
        }

        files.Where(s => !s.StartsWith("http")).ForEach(s =>
        {
            if (Directory.Exists(s))
            {
                GetFilesRecurs(s);
            }
            else
            {
                fileList.Add(s);
            }
        });

        if (!fileList.Any())
        {
            return new Dictionary<string, string>();
        }

        var dirName = new string(fileList.First().Substring(0, fileList.Min(s => s.Length))
            .TakeWhile((c, i) => fileList.All(s => s[i] == c)).ToArray());
        if (!Directory.Exists(dirName))
        {
            dirName = Directory.GetParent(dirName)?.FullName;
        }

        return fileList.ToDictionary(s => s, s => s.Substring(dirName.Length));
    }
}