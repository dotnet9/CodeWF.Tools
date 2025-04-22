using System;
using CodeWF.Tools.Models;

namespace CodeWF.Tools.Extensions;

public static class DoubleExtensions
{
    /// <summary>
    /// 根据指定数量的小数位数格式化字节数为易于理解的字符串，同时包含相应的单位
    /// </summary>
    /// <param name="bytes">要转换的字节数</param>
    /// <param name="decimalPlaces">保留的小数位数（默认2位）</param>
    /// <returns>格式化的字符串，如"1.23 GB"</returns>
    public static string FormatBytes(this double bytes, int decimalPlaces = 2)
    {
        // 处理负数情况
        var isNegative = bytes < 0;
        bytes = Math.Abs(bytes);

        var unit = 0;
        while (bytes >= 1024 && unit < Enum.GetValues(typeof(ByteUnit)).Length - 1)
        {
            bytes /= 1024;
            ++unit;
        }

        var formattedNumber = bytes.ToString($"F{decimalPlaces}");
        return $"{(isNegative ? "-" : "")}{formattedNumber} {(ByteUnit)unit}";
    }
}