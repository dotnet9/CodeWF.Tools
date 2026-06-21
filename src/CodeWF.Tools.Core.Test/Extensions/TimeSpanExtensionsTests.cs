using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Core.Test.Extensions;

public class TimeSpanExtensionsTests
{
    [Theory]
    [InlineData(0UL, "0:00:00.000")]
    [InlineData(1UL, "0:00:00.001")]
    [InlineData(3723004UL, "1:02:03.004")]
    [InlineData(90000000UL, "25:00:00.000")]
    public void FormatMilliseconds_ShouldReturnHourMinuteSecondMillisecondText(ulong milliseconds, string expected)
    {
        var result = milliseconds.FormatMilliseconds();

        Assert.Equal(expected, result);
    }
}
