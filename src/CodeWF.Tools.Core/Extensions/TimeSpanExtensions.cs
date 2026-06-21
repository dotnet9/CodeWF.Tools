using System;

namespace CodeWF.Tools.Extensions;

public static class TimeSpanExtensions
{
    public static string FormatMilliseconds(this double milliseconds)
    {
        return TimeSpan.FromMilliseconds(milliseconds).ToHourMinuteSecondMillisecondText();
    }

    public static string FormatMilliseconds(this long milliseconds)
    {
        return TimeSpan.FromMilliseconds(milliseconds).ToHourMinuteSecondMillisecondText();
    }

    public static string FormatMilliseconds(this ulong milliseconds)
    {
        return TimeSpan.FromMilliseconds(milliseconds).ToHourMinuteSecondMillisecondText();
    }

    public static string ToHourMinuteSecondMillisecondText(this TimeSpan timeSpan)
    {
        var hours = (timeSpan.Days * 24 + timeSpan.Hours).ToString();
        var minutes = timeSpan.Minutes.ToString().PadLeft(2, '0');
        var seconds = timeSpan.Seconds.ToString().PadLeft(2, '0');
        var milliseconds = timeSpan.Milliseconds.ToString().PadLeft(3, '0');

        return $"{hours}:{minutes}:{seconds}.{milliseconds}";
    }
}
