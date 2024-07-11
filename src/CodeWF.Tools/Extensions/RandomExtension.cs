using System;

namespace CodeWF.Tools.Extensions;

public static class RandomExtension
{
    public static int GetInt(int start, int end)
    {
        var tick = DateTimeOffset.UtcNow.Ticks;
        var seed = (int)(tick & 0xFFFFFFFFL) | (int)(tick >> 32);
        return new Random(seed).Next(start, end);
    }
}