using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Core.Test;

public class DateTimeExtensionsTests
{
    [Fact]
    public void GetTimeIntervalMilliseconds_DateTime_DefaultOffset()
    {
        var start = DateTime.Now;
        var end = start.AddMilliseconds(1000);
        var interval = end.GetTimeIntervalMilliseconds(start);
        Assert.Equal(1000u, interval);
    }

    [Fact]
    public void GetTimeIntervalMilliseconds_DateTime_SpecifiedOffset()
    {
        var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);
        var end = start.AddMilliseconds(1000);
        var offset = TimeSpan.FromHours(1);
        var interval = end.GetTimeIntervalMilliseconds(offset, start);
        Assert.Equal(1000u, interval);
    }

    [Fact]
    public void GetTimeIntervalMilliseconds_DateTimeOffset()
    {
        var start = DateTimeOffset.Now;
        var end = start.AddMilliseconds(1000);
        var interval = end.GetTimeIntervalMilliseconds(start);
        Assert.Equal(1000u, interval);
    }

    [Fact]
    public void GetEndDateTime_DefaultOffset()
    {
        var start = DateTime.Now;
        var milliseconds = 1000u;
        var end = start.GetEndDateTime(milliseconds);
        Assert.Equal(start.AddMilliseconds(milliseconds), end);
    }

    [Fact]
    public void GetEndDateTime_SpecifiedOffset()
    {
        var start = DateTime.Now;
        var milliseconds = 1000u;
        var offset = TimeSpan.FromHours(1);

        var calculatedEnd = start.GetEndDateTime(offset, milliseconds);

        var startOffset = CreateDateTimeOffset(start, offset);
        var expectedEnd = startOffset.AddMilliseconds(milliseconds).DateTime;

        Assert.Equal(expectedEnd, calculatedEnd);
    }

    [Fact]
    public void GetEndDateTimeOffset()
    {
        var start = DateTimeOffset.Now;
        var milliseconds = 1000u;
        var end = start.GetEndDateTimeOffset(milliseconds);
        Assert.Equal(start.AddMilliseconds(milliseconds), end);
    }

    private static DateTimeOffset CreateDateTimeOffset(DateTime dt, TimeSpan offset)
    {
        if (dt.Kind == DateTimeKind.Local)
        {
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        }

        return new DateTimeOffset(dt, offset);
    }
}