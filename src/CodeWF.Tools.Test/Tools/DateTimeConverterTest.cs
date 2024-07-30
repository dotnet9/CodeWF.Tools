// ReSharper disable once CheckNamespace
namespace CodeWF.Tools.Tests;

public class DateTimeConverterTest
{
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
}