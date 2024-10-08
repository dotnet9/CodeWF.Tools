﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CodeWF.Tools.Extensions;

public static class StringExtensions
{
    public static string Join(this IEnumerable<string> strs, string separate = ", ", bool removeEmptyEntry = false) =>
        string.Join(separate, removeEmptyEntry ? strs.Where(s => !string.IsNullOrEmpty(s)) : strs);

    public static string Join<T>(this IEnumerable<T> strs, string separate = ", ", bool removeEmptyEntry = false) =>
        string.Join(separate, removeEmptyEntry ? strs.Where(t => t != null) : strs);

    /// <summary>
    /// 字符串转时间
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string value)
    {
        DateTime.TryParse(value, out var result);
        return result;
    }

    /// <summary>
    /// 使用浏览器打开网址
    /// </summary>
    /// <param name="link"></param>
    public static void OpenBrowserForVisitSite(this string link)
    {
        var param = new ProcessStartInfo { FileName = link, UseShellExecute = true, Verb = "open" };
        Process.Start(param);
    }
}