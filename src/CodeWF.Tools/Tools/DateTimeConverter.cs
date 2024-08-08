using System;
using System.Globalization;
using System.Text.RegularExpressions;
// ReSharper disable once CheckNamespace
using CodeWF.Tools.Extensions;

namespace CodeWF.Tools;

public class DateTimeConverter
{
    private static readonly Regex ISO8601_REGEX = new Regex(
        @"^([+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24:?00)([.,]\d+(?!:))?)?(\17[0-5]\d([.,]\d+)?)?([zZ]|([+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$");

    private static readonly Regex ISO9075_REGEX =
        new Regex(
            @"^([0-9]{4})-([0-9]{2})-([0-9]{2}) ([0-9]{2}):([0-9]{2}):([0-9]{2})(\.[0-9]{1,6})?(([+-])([0-9]{2}):([0-9]{2})|Z)?$");

    private static readonly Regex RFC3339_REGEX =
        new Regex(
            @"^([0-9]{4})-([0-9]{2})-([0-9]{2})T([0-9]{2}):([0-9]{2}):([0-9]{2})(\.[0-9]{1,9})?(([+-])([0-9]{2}):([0-9]{2})|Z)$");

    private static readonly Regex RFC7231_REGEX =
        new Regex(@"^[A-Za-z]{3},\s[0-9]{2}\s[A-Za-z]{3}\s[0-9]{4}\s[0-9]{2}:[0-9]{2}:[0-9]{2}\sGMT$");

    private static readonly Regex EXCEL_FORMAT_REGEX = new Regex(@"^-?\d+(\.\d+)?$");

    public static Func<string?, bool> CreateRegexMatcher(Regex regex)
    {
        return (date) => !string.IsNullOrWhiteSpace(date) && regex.IsMatch(date);
    }

    public static Func<string?, bool> IsISO8601DateTimeString = CreateRegexMatcher(ISO8601_REGEX);
    public static Func<string?, bool> IsISO9075DateString = CreateRegexMatcher(ISO9075_REGEX);
    public static Func<string?, bool> IsRFC3339DateString = CreateRegexMatcher(RFC3339_REGEX);
    public static Func<string?, bool> IsRFC7231DateString = CreateRegexMatcher(RFC7231_REGEX);
    public static Func<string?, bool> IsUnixTimestamp = CreateRegexMatcher(new Regex(@"^[0-9]{1,10}$"));
    public static Func<string?, bool> IsTimestamp = CreateRegexMatcher(new Regex(@"^[0-9]{1,13}$"));
    public static Func<string?, bool> IsMongoObjectId = CreateRegexMatcher(new Regex(@"^[0-9a-fA-F]{24}$"));

    public static Func<string?, bool> IsExcelFormat = CreateRegexMatcher(EXCEL_FORMAT_REGEX);

    public static bool IsUTCDateString(string? date)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            return false;
        }

        return DateTime.TryParseExact(date, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    public static string DateToExcelFormat(DateTime date)
    {
        TimeSpan timeSpan = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
        double days = timeSpan.TotalDays;
        return ((days + 25569)).ToString(CultureInfo.InvariantCulture);
    }

    public static DateTime ExcelFormatToDate(string excelFormat)
    {
        if (!double.TryParse(excelFormat, out double days))
        {
            throw new ArgumentException("Invalid Excel format.", nameof(excelFormat));
        }

        return new DateTime(1899, 12, 30, 0, 0, 0, DateTimeKind.Utc).AddDays(days);
    }

    public static DateTime ExcelFormatToDate(double excelDays)
    {
        TimeSpan timeSpan = TimeSpan.FromDays(excelDays - 25569);
        return new DateTime(1970, 1, 1, 0, 0, 0).Add(timeSpan);
    }
}