using SharpCompress.Common;
using System.Collections.Generic;

namespace CodeWF.Tools.FileExtensions;

/// <summary>
/// 7z压缩
/// </summary>
public interface ISevenZipCompressor
{
    /// <summary>
    /// 解压文件，自动检测压缩包类型
    /// </summary>
    /// <param name="compressedFile">rar文件</param>
    /// <param name="dir">解压到...</param>
    void Decompress(string compressedFile, string dir = "");

    /// <summary>
    /// 压缩多个文件
    /// </summary>
    /// <param name="files">多个文件路径，文件或文件夹</param>
    /// <param name="zipFile">压缩到...</param>
    /// <param name="rootDir">压缩包内部根文件夹</param>
    /// <param name="archiveType"></param>
    void Zip(IEnumerable<string> files, string zipFile, string rootDir = "", ArchiveType archiveType = ArchiveType.Zip);

    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="dir">文件夹</param>
    /// <param name="zipFile">压缩到...</param>
    /// <param name="rootDir">压缩包内部根文件夹</param>
    /// <param name="archiveType"></param>
    public void Zip(string dir, string zipFile, string rootDir = "", ArchiveType archiveType = ArchiveType.Zip);
}