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
        if (!string.IsNullOrWhiteSpace(exePath) && File.Exists(exePath))
        {
            return new FileInfo(exePath).LastWriteTime;
        }

        return default;
    }
}