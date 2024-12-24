using CodeWF.Tools.Exports;
using CodeWF.Tools.FileExtensions;
using System.Data;

namespace CodeWF.Tools.Test;

public class ExportTest
{
    [Fact]
    public void Test_ExportCsv_Success()
    {
        var file = "1.csv";
        FileHelper.DeleteFileIfExist(file);
        Assert.False(File.Exists(file));

        var data = GetData();
        data.Export(file, out var errorMsg);
        Assert.True(File.Exists(file));

        FileHelper.DeleteFileIfExist(file);
    }

    [Fact]
    public void Test_ExportXlsx_Success()
    {
        var file = "1.xlsx";
        FileHelper.DeleteFileIfExist(file);
        Assert.False(File.Exists(file));

        var data = GetData();
        data.Export(file, out var errorMsg);
        Assert.True(File.Exists(file));

        FileHelper.DeleteFileIfExist(file);
    }

    private DataTable GetData()
    {
        DataTable dataTable = new DataTable("SampleTable");
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Age", typeof(int));

        dataTable.Rows.Add("Alice", 25);
        dataTable.Rows.Add("Bob", 30);
        return dataTable;
    }
}