using System.IO.Compression;
using System.Text;
using CodeWF.Tools.FileExtensions;

namespace CodeWF.Tools.Test;

public class SevenZipCompressorTest
{
    [Fact]
    public void Decompress_ShouldRejectPathTraversalEntry()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory, "malicious.zip");
            CreateZip(archivePath, (archive, entryName) =>
            {
                var entry = archive.CreateEntry(entryName);
                using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
                writer.Write("unexpected");
            }, "../escaped.txt");

            var outputDirectory = Path.Combine(tempDirectory, "output");
            var escapedPath = Path.Combine(tempDirectory, "escaped.txt");
            using var httpClient = new HttpClient();
            var compressor = new SevenZipCompressor(httpClient);

            Assert.Throws<InvalidDataException>(() => compressor.Decompress(archivePath, outputDirectory));
            Assert.False(File.Exists(escapedPath));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public void Decompress_ShouldHonorIgnoreEmptyDirectory()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory, "entries.zip");
            CreateZip(archivePath, (archive, entryName) => archive.CreateEntry(entryName), "empty/");
            using var httpClient = new HttpClient();
            var compressor = new SevenZipCompressor(httpClient);

            var ignoredDirectory = Path.Combine(tempDirectory, "ignored");
            compressor.Decompress(archivePath, ignoredDirectory, ignoreEmptyDir: true);
            Assert.False(Directory.Exists(Path.Combine(ignoredDirectory, "empty")));

            var extractedDirectory = Path.Combine(tempDirectory, "extracted");
            compressor.Decompress(archivePath, extractedDirectory, ignoreEmptyDir: false);
            Assert.True(Directory.Exists(Path.Combine(extractedDirectory, "empty")));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public void Zip_ShouldUseRelativeEntryPathForSingleFile()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "single.txt");
            File.WriteAllText(sourcePath, "content");
            var archivePath = Path.Combine(tempDirectory, "single.zip");
            var outputDirectory = Path.Combine(tempDirectory, "output");
            using var httpClient = new HttpClient();
            var compressor = new SevenZipCompressor(httpClient);

            compressor.Zip(new[] { sourcePath }, archivePath);
            compressor.Decompress(archivePath, outputDirectory);

            Assert.Equal("content", File.ReadAllText(Path.Combine(outputDirectory, "single.txt")));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"codewf-compressor-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static void CreateZip(string path, Action<ZipArchive, string> addEntry, string entryName)
    {
        using var stream = File.Create(path);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
        addEntry(archive, entryName);
    }
}
