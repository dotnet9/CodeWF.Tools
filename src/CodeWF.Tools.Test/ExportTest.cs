using CodeWF.Tools.Exports;
using CodeWF.Tools.FileExtensions;
using System.Data;
using System.Text;
using DataTableExtensions = CodeWF.Tools.Exports.DataTableExtensions;

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
        data.Export(file, Encoding.Default,  out var errorMsg);
        Assert.True(File.Exists(file));

        var importResult = DataTableExtensions.Import(file, out errorMsg, out var newData);
        Assert.True(importResult);
        CheckData(data, newData);

        FileHelper.DeleteFileIfExist(file);
    }

    [Fact]
    public void Test_ExportXlsx_Success()
    {
        var file = "1.xlsx";
        FileHelper.DeleteFileIfExist(file);
        Assert.False(File.Exists(file));

        var data = GetData();
        data.Export(file, Encoding.Default,  out var errorMsg);
        Assert.True(File.Exists(file));

        var importResult = DataTableExtensions.Import(file, out errorMsg, out var newData);
        Assert.True(importResult);
        CheckData(data, newData);

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

    private void CheckData(DataTable oldData, DataTable newData)
    {
        Assert.Equal(oldData.Columns.Count, newData.Columns.Count);
        Assert.Equal(oldData.Rows.Count, newData.Rows.Count);
        for (var i = 0; i < oldData.Columns.Count; i++)
        {
            Assert.Equal(oldData.Columns[i].ToString(), newData.Columns[i].ToString());
        }

        for (var i = 0; i < oldData.Rows.Count; i++)
        {
            for (var j = 0; j < oldData.Columns.Count; j++)
            {
                Assert.Equal(oldData.Rows[i][j].ToString(), newData.Rows[i][j].ToString());
            }
        }
    }
}