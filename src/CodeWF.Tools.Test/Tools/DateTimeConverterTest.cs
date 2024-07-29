// ReSharper disable once CheckNamespace
namespace CodeWF.Tools.Tests;

public class DateTimeConverterTest
{
    [Fact]
    public void ShouldReturnTrueForValidISO8601DateStringgs()
    {
        Assert.True(DateTimeConverter.IsISO8601DateTimeString("2021-01-01T00:00:00.000Z"));
    }
}