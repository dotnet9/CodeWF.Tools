using CodeWF.Tools.FileExtensions;
using System.Reflection;

namespace CodeWF.Tools.Test;

[TestClass]
public class FileHelperTest
{
    [TestMethod]
    public void Test_GetTempFileName_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        File.AppendAllText(tempFilePath, "test");
        FileHelper.DeleteFileIfExist(tempFilePath);
        Assert.IsFalse(File.Exists(tempFilePath));
    }


    [TestMethod]
    public void Test_DeleteFileIfExist_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        Assert.IsFalse(File.Exists(tempFilePath));
        FileHelper.DeleteFileIfExist(tempFilePath);
    }

    [TestMethod]
    public void Test_OpenFolder_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        var tempDir = Path.GetDirectoryName(tempFilePath);
        Assert.IsTrue(Directory.Exists(tempDir));
        FileHelper.OpenFolder(tempDir!);
    }

    [TestMethod]
    public void Test_OpenFolderAndSelectFile_Success()
    {
        var tempFilePath = FileHelper.GetTempFileName(Assembly.GetExecutingAssembly().GetName().Name!);
        File.AppendAllText(tempFilePath, "test");
        Assert.IsTrue(File.Exists(tempFilePath));
        FileHelper.OpenFolderAndSelectFile(tempFilePath);
        FileHelper.DeleteFileIfExist(tempFilePath);
        Assert.IsFalse(File.Exists(tempFilePath));
    }
}