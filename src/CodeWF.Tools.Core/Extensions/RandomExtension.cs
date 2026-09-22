using System;

namespace CodeWF.Tools.Extensions;

public static class RandomExtension
{
    public static int GetInt(int start, int end)
    {
        return Random.Shared.Next(start, end);
    }
}
