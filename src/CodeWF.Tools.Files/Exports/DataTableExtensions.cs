using CsvHelper;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeWF.Tools.Exports;

public static class DataTableExtensions
{
    public static bool Export(this DataTable dataTable, string saveFilePath, Encoding encoding, out string errorMsg,
        bool containColumnHeader = true)
    {
        if (saveFilePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return dataTable.ExportToCsv(saveFilePath, encoding, out errorMsg, containColumnHeader);
        }

        if (saveFilePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return dataTable.ExportToXlsx(saveFilePath, out errorMsg, containColumnHeader);
        }

        throw new Exception("Currently only supports .csv and .xlsx file exports");
    }


    public static bool ExportToCsv(this DataTable dataTable, string saveFilePath, Encoding encoding, out string errorMsg,
        bool containColumnHeader = true)
    {
        errorMsg = "";
        try
        {
            using var writer = new StreamWriter(saveFilePath, false, encoding);
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
            MiniExcel.SaveAs(saveFilePath, dataTable, printHeader: containColumnHeader, overwriteFile: overwriteFile);
            return true;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
    }


    public static bool Import(string openFilePath, out string errorMsg, out DataTable? dataTable,
        bool containColumnHeader = true)
    {
        if (openFilePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return ImportFromCsv(openFilePath, out errorMsg, out dataTable, containColumnHeader);
        }

        if (openFilePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return ImportFromExcel(openFilePath, out errorMsg, out dataTable, containColumnHeader);
        }

        throw new Exception("Currently only supports .csv and .xlsx file imports");
    }

    public static bool ImportFromCsv(this string csvFilePath, out string errorMsg, out DataTable? dataTable,
        bool containColumnHeader = true)
    {
        errorMsg = "";
        dataTable = null;
        try
        {
            dataTable = new DataTable();
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            if (containColumnHeader && csv.Read())
            {
                csv.ReadHeader();
                var headers = csv.HeaderRecord;
                if (headers is null)
                {
                    throw new InvalidDataException("Can't read table headers");
                }

                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header);
                }
            }

            while (csv.Read())
            {
                if (dataTable.Columns.Count == 0)
                {
                    for (var i = 0; i < csv.ColumnCount; i++)
                    {
                        dataTable.Columns.Add(i.ToString(CultureInfo.InvariantCulture));
                    }
                }

                var row = dataTable.NewRow();
                for (var i = 0; i < csv.ColumnCount; i++)
                {
                    row[i] = csv.GetField(i);
                }

                dataTable.Rows.Add(row);
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool ImportFromExcel(this string excelFilePath, out string errorMsg, out DataTable? dataTable,
        bool containColumnHeader = true)
    {
        errorMsg = "";
        dataTable = null;
        try
        {
            var excelData = MiniExcel.Query(excelFilePath, containColumnHeader).ToList();
            if (excelData.Count == 0)
            {
                return false;
            }

            dataTable = new DataTable();
            var columnNames = (excelData.First() as IDictionary<string, object>)?.Keys.ToArray();
            if (columnNames == null)
            {
                throw new Exception("Can't read table columns");
            }

            var targetColumnNames = new string[columnNames.Length];
            if (containColumnHeader)
            {
                for (var i = 0; i < columnNames.Length; i++)
                {
                    targetColumnNames[i] = columnNames[i];
                    dataTable.Columns.Add(targetColumnNames[i]);
                }
            }
            else
            {
                for (var i = 0; i < columnNames.Length; i++)
                {
                    targetColumnNames[i] = i.ToString(CultureInfo.InvariantCulture);
                    dataTable.Columns.Add(targetColumnNames[i]);
                }
            }

            for (var i = 0; i < excelData.Count; i++)
            {
                var rowDatas = excelData[i] as IDictionary<string, object>;
                if (rowDatas is null)
                {
                    continue;
                }

                var row = dataTable.NewRow();
                for (var columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
                {
                    var sourceColumnName = columnNames[columnIndex];
                    var targetColumnName = targetColumnNames[columnIndex];
                    row[targetColumnName] = rowDatas.TryGetValue(sourceColumnName, out var val) && val is not null
                        ? val
                        : DBNull.Value;
                }

                dataTable.Rows.Add(row);
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
    }
}
