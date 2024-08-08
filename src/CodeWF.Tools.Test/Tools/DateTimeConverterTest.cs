// ReSharper disable once CheckNamespace
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeWF.Tools.Tests;

public class DateTimeConverterTest
{
    #region isISO8601DateTimeString

    [Fact(DisplayName = "should return true for valid ISO 8601 date strings")]
    public void ShouldReturnTrueForValidISO8601DateStringgs()
    {
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("2021-01-01T00:00:00.000Z"));
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("2023-04-12T14:56:00+01:00"));
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("20230412T145600+0100"));
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("20230412T145600Z"));
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("2016-02-01"));
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("2016"));
    }

    [Fact(DisplayName = "should return false for invalid ISO 8601 date strings")]
    public void ShouldReturnFalseForInvalidISO8601DateStrings()
    {
        Assert.False(DateTimeConverter.IsISO8601DateTimeString(null));
        Assert.False(DateTimeConverter.IsISO8601DateTimeString(""));
        Assert.False(DateTimeConverter.IsISO8601DateTimeString("qsdqsd"));
        Assert.False(DateTimeConverter.IsISO8601DateTimeString("2016-02-01-"));
        Assert.False(DateTimeConverter.IsISO8601DateTimeString("2021-01-01T00:00:00."));
    }

    #endregion

    #region isISO9075DateString

    [Fact(DisplayName = "should return true for valid ISO 9075 date strings")]
    public void ShouldReturnTrueForValidISO9075DateStrings()
    {
        Assert.True(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00Z"));
        Assert.True(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00.123456Z"));
        Assert.True(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00+01:00"));
        Assert.True(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00-05:00"));
    }

    [Fact(DisplayName = "should return false for invalid ISO 9075 date strings")]
    public void ShouldReturnFalseForInvalidISO9075DateStrings()
    {
        Assert.False(DateTimeConverter.IsISO9075DateString("2022/01/01T12:00:00Z"));
        Assert.False(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00.123456789Z"));
        Assert.False(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00+1:00"));
        Assert.False(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00-05:"));
        Assert.False(DateTimeConverter.IsISO9075DateString("2022-01-01 12:00:00-05:00:00"));
        Assert.False(DateTimeConverter.IsISO9075DateString("2022-01-01"));
        Assert.False(DateTimeConverter.IsISO9075DateString("12:00:00Z"));
        Assert.False(DateTimeConverter.IsISO9075DateString("2022-01-01T12:00:00Zfoo"));
    }

    #endregion

    #region isRFC3339DateString

    [Fact(DisplayName = "should return true for valid RFC 3339 date strings")]
    public void ShouldReturnTrueForValidRFC3339DateStrings()
    {
        Assert.True(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00Z"));
        Assert.True(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00.123456789Z"));
        Assert.True(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00.123456789+01:00"));
        Assert.True(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00-05:00"));
    }

    [Fact(DisplayName = "should return false for invalid RFC 3339 date strings")]
    public void ShouldReturnFalseForInvalidRFC3339DateStrings()
    {
        Assert.False(DateTimeConverter.IsRFC3339DateString("2022/01/01T12:00:00Z"));
        Assert.False(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00.123456789+1:00"));
        Assert.False(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00-05:"));
        Assert.False(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00-05:00:00"));
        Assert.False(DateTimeConverter.IsRFC3339DateString("2022-01-01"));
        Assert.False(DateTimeConverter.IsRFC3339DateString("12:00:00Z"));
        Assert.False(DateTimeConverter.IsRFC3339DateString("2022-01-01T12:00:00Zfoo"));
    }

    #endregion

    #region isRFC7231DateString

    [Fact(DisplayName = "should return true for valid RFC 7231 date strings")]
    public void ShouldReturnTrueForValidRFC7231DateStrings()
    {
        Assert.True(DateTimeConverter.IsRFC7231DateString("Sun, 06 Nov 1994 08:49:37 GMT"));
        Assert.True(DateTimeConverter.IsRFC7231DateString("Tue, 22 Apr 2014 07:00:00 GMT"));
    }

    [Fact(DisplayName = "should return false for invalid RFC 7231 date strings")]
    public void ShouldReturnFalseForInvalidRFC7231DateStrings()
    {
        Assert.False(DateTimeConverter.IsRFC7231DateString("06 Nov 1994 08:49:37 GMT"));
        Assert.False(DateTimeConverter.IsRFC7231DateString("Sun, 06 Nov 94 08:49:37 GMT"));
        Assert.False(DateTimeConverter.IsRFC7231DateString("Sun, 06 Nov 1994 8:49:37 GMT"));
        Assert.False(DateTimeConverter.IsRFC7231DateString("Sun, 06 Nov 1994 08:49:37 GMT-0500"));
        Assert.False(DateTimeConverter.IsRFC7231DateString("Sun, 06 November 1994 08:49:37 GMT"));
        Assert.False(DateTimeConverter.IsRFC7231DateString("Sunday, 06 Nov 1994 08:49:37 GMT"));
        Assert.False(DateTimeConverter.IsRFC7231DateString("06 Nov 1994"));
    }

    #endregion

    #region isUnixTimestamp

    [Fact(DisplayName = "should return true for valid Unix timestamps")]
    public void ShouldReturnTrueForValidUnixTimestamps()
    {
        Assert.True(DateTimeConverter.IsUnixTimestamp("1649789394"));
        Assert.True(DateTimeConverter.IsUnixTimestamp("1234567890"));
        Assert.True(DateTimeConverter.IsUnixTimestamp("0"));
    }

    [Fact(DisplayName = "should return false for invalid Unix timestamps")]
    public void ShouldReturnFalseForInvalidUnixTimestamps()
    {
        Assert.False(DateTimeConverter.IsUnixTimestamp("foo"));
        Assert.False(DateTimeConverter.IsUnixTimestamp(""));
    }

    #endregion

    #region isTimestamp

    [Fact(DisplayName = "should return true for valid Unix timestamps in milliseconds")]
    public void ShouldReturnTrueForValidUnixTmestampsInMilliseconds()
    {
        Assert.True(DateTimeConverter.IsTimestamp("1649792026123"));
        Assert.True(DateTimeConverter.IsTimestamp("1234567890000"));
        Assert.True(DateTimeConverter.IsTimestamp("0"));
    }

    [Fact(DisplayName = "should return false for invalid Unix timestamps in milliseconds")]
    public void ShouldTeturnFalseForInvalidUnixTimestampsInMilliseconds()
    {
        Assert.False(DateTimeConverter.IsTimestamp("foo"));
        Assert.False(DateTimeConverter.IsTimestamp(""));
    }

    #endregion

    #region isUTCDateString

    [Fact(DisplayName = "should return true for valid UTC date strings")]
    public void ShouldReturnTrueForValidUTCDateStrings()
    {
        Assert.True(DateTimeConverter.IsUTCDateString("Sun, 06 Nov 1994 08:49:37 GMT"));
        Assert.True(DateTimeConverter.IsUTCDateString("Tue, 22 Apr 2014 07:00:00 GMT"));
    }

    [Fact(DisplayName = "should return false for invalid UTC date strings")]
    public void ShouldReturnFalseForInvalidUTCDateStrings()
    {
        Assert.False(DateTimeConverter.IsUTCDateString("06 Nov 1994 08:49:37 GMT"));
        Assert.False(DateTimeConverter.IsUTCDateString("16497920261"));
        Assert.False(DateTimeConverter.IsUTCDateString("foo"));
        Assert.False(DateTimeConverter.IsUTCDateString(""));
    }

    #endregion

    #region isMongoObjectId

    [Fact(DisplayName = "should return true for valid Mongo ObjectIds")]
    public void ShouldReturnTrueForValidMongoObjectIds()
    {
        Assert.True(DateTimeConverter.IsMongoObjectId("507f1f77bcf86cd799439011"));
        Assert.True(DateTimeConverter.IsMongoObjectId("507f1f77bcf86cd799439012"));
    }

    [Fact(DisplayName = "should return false for invalid Mongo ObjectIds")]
    public void ShouldReturnFalseForInvalidMongoObjectIds()
    {
        Assert.False(DateTimeConverter.IsMongoObjectId("507f1f77bcf86cd79943901"));
        Assert.False(DateTimeConverter.IsMongoObjectId("507f1f77bcf86cd79943901z"));
        Assert.False(DateTimeConverter.IsMongoObjectId("foo"));
        Assert.False(DateTimeConverter.IsMongoObjectId(""));
    }

    #endregion

    #region isExcelFormat

    [Fact(DisplayName = "an Excel format string is a floating number that can be negative")]
    public void AnExcelFormatStringIsAFloatingNumberThatCanBeNegative()
    {
        Assert.True(DateTimeConverter.IsExcelFormat("0"));
        Assert.True(DateTimeConverter.IsExcelFormat("1"));
        Assert.True(DateTimeConverter.IsExcelFormat("1.1"));
        Assert.True(DateTimeConverter.IsExcelFormat("-1.1"));
        Assert.True(DateTimeConverter.IsExcelFormat("-1"));


        Assert.False(DateTimeConverter.IsExcelFormat(""));
        Assert.False(DateTimeConverter.IsExcelFormat("foo"));
        Assert.False(DateTimeConverter.IsExcelFormat("1.1.1"));

    }

    #endregion

    #region dateToExcelFormat

    [Fact(DisplayName = "a date in Excel format is the number of days since 01/01/1900")]
    public void ADateInExcelFormatIsTheNumberOfDaysSince01_01_1900()
    {
        Assert.Equal("42510", DateTimeConverter.DateToExcelFormat(DateTime.Parse("2016-05-20T00:00:00.000Z")));
        Assert.Equal("42510.5", DateTimeConverter.DateToExcelFormat(DateTime.Parse("2016-05-20T12:00:00.000Z")));
        Assert.Equal("45230.39312987268", DateTimeConverter.DateToExcelFormat(DateTime.Parse("2023-10-31T09:26:06.421Z")));
        Assert.Equal("25569", DateTimeConverter.DateToExcelFormat(DateTime.Parse("1970-01-01T00:00:00.000Z")));
        Assert.Equal("-36522", DateTimeConverter.DateToExcelFormat(DateTime.Parse("1800-01-01T00:00:00.000Z")));
    }

    #endregion

    #region excelFormatToDate

    [Fact(DisplayName = "a date in Excel format is the number of days since 01/01/1900")]
    public void ExcelFormatToDate()
    {
        Assert.Equal(DateTime.Parse("1899-12-30T00:00:00.000Z", null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), DateTimeConverter.ExcelFormatToDate("0"));
        Assert.Equal(DateTime.Parse("1899-12-31T00:00:00.000Z", null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), DateTimeConverter.ExcelFormatToDate("1"));
        Assert.Equal(DateTime.Parse("1900-01-01T00:00:00.000Z", null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), DateTimeConverter.ExcelFormatToDate("2"));
        Assert.Equal(DateTime.Parse("1911-08-12T10:10:50.880Z", null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), DateTimeConverter.ExcelFormatToDate("4242.4242"));
        Assert.Equal(DateTime.Parse("2017-01-03T05:25:49.607Z", null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), DateTimeConverter.ExcelFormatToDate("42738.22626859954"));
        Assert.Equal(DateTime.Parse("1897-04-04T00:00:00.000Z", null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime(), DateTimeConverter.ExcelFormatToDate("-1000"));
    }

    #endregion
}