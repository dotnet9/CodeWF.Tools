using CodeWF.Tools.FileExtensions;
using System.Reflection;

namespace CodeWF.Tools.Test;

public class FileHelperTest
{
    [Fact]
    public void Test_GetTempFileName_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        File.AppendAllText(tempFilePath, "test");
        FileHelper.DeleteFileIfExist(tempFilePath);
        Assert.False(File.Exists(tempFilePath));
    }


    [Fact]
    public void Test_DeleteFileIfExist_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        Assert.False(File.Exists(tempFilePath));
        FileHelper.DeleteFileIfExist(tempFilePath);
    }

    [Fact]
    public void Test_OpenFolder_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        var tempDir = Path.GetDirectoryName(tempFilePath);
        Assert.True(Directory.Exists(tempDir));
        FileHelper.OpenFolder(tempDir!);
    }

    [Fact]
    public void Test_OpenFolderAndSelectFile_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        File.AppendAllText(tempFilePath, "test");
        Assert.True(File.Exists(tempFilePath));
        FileHelper.OpenFolderAndSelectFile(tempFilePath);
        FileHelper.DeleteFileIfExist(tempFilePath);
        Assert.False(File.Exists(tempFilePath));
    }
}