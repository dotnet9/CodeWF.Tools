using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Core.Test;

public class DateTimeExtensionsTests
{
    [Fact]
    public void GetTimeIntervalMilliseconds_DefaultOffset()
    {
        var startDt = new DateTime(2025, 1, 1);
        var endDt = new DateTime(2025, 1, 2);
        var result = endDt.GetTimeIntervalMilliseconds(startDt);
        var expected = (uint)(TimeSpan.FromDays(1).TotalMilliseconds);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetTimeIntervalMilliseconds_SpecifiedOffset()
    {
        var offset = TimeSpan.FromHours(8);
        var startDt = new DateTime(2025, 1, 1);
        var endDt = new DateTime(2025, 1, 2);
        var result = endDt.GetTimeIntervalMilliseconds(offset, startDt);
        var expected = (uint)(TimeSpan.FromDays(1).TotalMilliseconds);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetTimeIntervalMilliseconds_DateTimeOffset()
    {
        var startDt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var endDt = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var result = endDt.GetTimeIntervalMilliseconds(startDt);
        var expected = (uint)(TimeSpan.FromDays(1).TotalMilliseconds);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetTimeIntervalMilliseconds_Overflow()
    {
        var startDt = new DateTime(2025, 1, 1);
        var endDt = new DateTime(2025, 3, 1);
        Assert.Throws<OverflowException>(() => endDt.GetTimeIntervalMilliseconds(startDt));
    }
}