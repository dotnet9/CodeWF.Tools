namespace CodeWF.Tools.Hardware;

public static class SystemInfo
{
    /// <summary>
    /// 根据指定数量的小数位数格式化字节数为易于理解的字符串，同时包含相应的单位
    /// </summary>
    /// <param name="bytes">要转换的字节数</param>
    /// <param name="decimalPlaces">保留的小数位数（默认2位）</param>
    /// <returns>格式化的字符串，如"1.23 GB"</returns>
    public static string FormatBytes(this double bytes, int decimalPlaces = 2)
    {
        var unit = 0;
        while (bytes > 1024)
        {
            bytes /= 1024;
            ++unit;
        }

        return bytes.ToString($"F{decimalPlaces}{(ByteUnit)unit}");
    }
}