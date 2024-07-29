using System;
using System.Text.RegularExpressions;
// ReSharper disable once CheckNamespace
using CodeWF.Tools.Extensions;

namespace CodeWF.Tools;

public class DateTimeConverter
{
    private const string ISO8601_REGEX =
        @"^([+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24:?00)([.,]\d+(?!:))?)?(\17[0-5]\d([.,]\d+)?)?([zZ]|([+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$";

    private const string ISO9075_REGEX =
        @"^([0-9]{4})-([0-9]{2})-([0-9]{2}) ([0-9]{2}):([0-9]{2}):([0-9]{2})(\.[0-9]{1,6})?(([+-])([0-9]{2}):([0-9]{2})|Z)?$";

    private const string RFC3339_REGEX =
        @"^([0-9]{4})-([0-9]{2})-([0-9]{2})T([0-9]{2}):([0-9]{2}):([0-9]{2})(\.[0-9]{1,9})?(([+-])([0-9]{2}):([0-9]{2})|Z)$";

    private const string RFC7231_REGEX =
        @"^[A-Za-z]{3},\s[0-9]{2}\s[A-Za-z]{3}\s[0-9]{4}\s[0-9]{2}:[0-9]{2}:[0-9]{2}\sGMT$";

    private const string EXCEL_FORMAT_REGEX = @"^-?\d+(\.\d+)?$";

    public static bool IsISO8601DateTimeString(string? date)
    {
        return !date.IsNullOrEmpty() && Regex.IsMatch(date!, ISO8601_REGEX);
    }

    public static bool IsUTCDateString(string? date)
    {
        if (date.IsNullOrEmpty())
        {
            return false;
        }

        try
        {
            return DateTimeOffset.Parse(date).ToString() == date;
        }
        catch
        {
            return false;
        }
    }

    public static string DateToExcelFormat(DateTimeOffset date)
    {
        return date.ToUnixTimeMilliseconds() / (1000 * 60 * 60 * 24) + 25569 + "";
    }

    //public DateTimeOffset ExcelFormatToDate(string excelFormat)
    //{
    //    return DateTimeOffset.Parse((long.Parse(excelFormat) - 25569) * 86400 * 1000);
    //}
}