using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CodeWF.Tools.Extensions;

public static class AssemblyExtensions
{
    public static string? Title(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        return attributes.Length > 0 ? ((AssemblyTitleAttribute)attributes[0]).Title : default;
    }

    public static string? Description(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        return attributes.Length > 0 ? ((AssemblyDescriptionAttribute)attributes[0]).Description : default;
    }

    public static string? Company(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        return attributes.Length > 0 ? ((AssemblyCompanyAttribute)attributes[0]).Company : default;
    }

    public static string? Product(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        return attributes.Length > 0 ? ((AssemblyProductAttribute)attributes[0]).Product : default;
    }

    public static string? Version(this Assembly? assembly)
    {
        return assembly?.GetName().Version?.ToString();
    }

    public static string? InformationalVersion(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
        return attributes.Length > 0
            ? ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion
            : default;
    }

    public static string? FileVersion(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
        return attributes.Length > 0 ? ((AssemblyFileVersionAttribute)attributes[0]).Version : default;
    }

    public static string? Copyright(this Assembly? assembly)
    {
        if (assembly == default)
        {
            return default;
        }

        var attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        return attributes.Length > 0 ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : default;
    }

    public static DateTime? CompileTime(this Assembly? assembly)
    {
        var exePath = Process.GetCurrentProcess()?.MainModule?.FileName;
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            return default;
        }
        
        const int PeHeaderOffset = 60;
        const int LinkerTimestampOffset = 8;
        const int ReadCount = 2048;
        var buffer = new byte[ReadCount];
        using (var s = new FileStream(exePath, FileMode.Open, FileAccess.Read))
        {
            s.Read(buffer, 0, ReadCount);
        }

        var i = BitConverter.ToInt32(buffer, PeHeaderOffset);
        var secondsSince1970 = BitConverter.ToInt32(buffer, i + LinkerTimestampOffset);
        var dt = new DateTime(1970, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddSeconds(secondsSince1970);
        return dt.ToLocalTime();
    }
}