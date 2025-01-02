using CsvHelper;
using MiniExcelLibs;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace CodeWF.Tools.Exports;

public static class DataTableExtensions
{
    public static bool Export(this DataTable dataTable, string saveFilePath, out string errorMsg,
        bool containColumnHeader = true)
    {
        if (saveFilePath.ToLower().EndsWith(".csv"))
        {
            return dataTable.ExportToCsv(saveFilePath, out errorMsg, containColumnHeader);
        }

        if (saveFilePath.ToLower().EndsWith(".xlsx"))
        {
            return dataTable.ExportToXlsx(saveFilePath, out errorMsg, containColumnHeader);
        }

        throw new Exception("Currently only supports .csv and .xlsx file exports");
    }


    public static bool ExportToCsv(this DataTable dataTable, string saveFilePath, out string errorMsg,
        bool containColumnHeader = true)
    {
        errorMsg = "";
        try
        {
            using var writer = new StreamWriter(saveFilePath, false, Encoding.Default);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            if (containColumnHeader)
            {
                foreach (DataColumn dc in dataTable.Columns)
                {
                    csv.WriteField(dc.ColumnName);
                }

                csv.NextRecord();
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (DataColumn dc in dataTable.Columns)
                {
                    csv.WriteField(dr[dc]);
                }

                csv.NextRecord();
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool ExportToXlsx(this DataTable dataTable, string saveFilePath, out string errorMsg,
        bool containColumnHeader = true, bool overwriteFile = true)
    {
        errorMsg = "";
        try
        {
            MiniExcel.SaveAs(saveFilePath, dataTable, overwriteFile: overwriteFile);
            return true;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
    }
}