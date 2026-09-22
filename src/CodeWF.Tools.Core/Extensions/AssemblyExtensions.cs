using System;
using System.Buffers.Binary;
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
        if (assembly is null)
        {
            return default;
        }

        var assemblyPath = GetAssemblyPath(assembly);
        if (assemblyPath is null)
        {
            return default;
        }

        return GetPeCompileTime(assemblyPath) ?? GetFileModificationTime(assemblyPath);
    }

    private static string? GetAssemblyPath(Assembly assembly)
    {
        try
        {
            var assemblyPath = assembly.Location;
            if (!string.IsNullOrWhiteSpace(assemblyPath) && File.Exists(assemblyPath))
            {
                return assemblyPath;
            }
        }
        catch
        {
            // Some dynamic and single-file assemblies do not expose a location.
        }

        try
        {
            var processPath = Process.GetCurrentProcess().MainModule?.FileName;
            return !string.IsNullOrWhiteSpace(processPath) && File.Exists(processPath)
                ? processPath
                : default;
        }
        catch
        {
            return default;
        }
    }

    private static DateTime? GetPeCompileTime(string assemblyPath)
    {
        try
        {
            const int DosHeaderSize = 64;
            const int PeHeaderOffsetPosition = 0x3C;
            const int PeHeaderPrefixSize = 12;
            using var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (stream.Length < DosHeaderSize)
            {
                return default;
            }

            Span<byte> dosHeader = stackalloc byte[DosHeaderSize];
            if (stream.Read(dosHeader) != DosHeaderSize || dosHeader[0] != 'M' || dosHeader[1] != 'Z')
            {
                return default;
            }

            var peHeaderOffset = BinaryPrimitives.ReadInt32LittleEndian(
                dosHeader[PeHeaderOffsetPosition..(PeHeaderOffsetPosition + sizeof(int))]);
            if (peHeaderOffset < 0 || peHeaderOffset > stream.Length - PeHeaderPrefixSize)
            {
                return default;
            }

            stream.Position = peHeaderOffset;
            Span<byte> peHeaderPrefix = stackalloc byte[PeHeaderPrefixSize];
            if (stream.Read(peHeaderPrefix) != PeHeaderPrefixSize ||
                peHeaderPrefix[0] != 'P' || peHeaderPrefix[1] != 'E' ||
                peHeaderPrefix[2] != 0 || peHeaderPrefix[3] != 0)
            {
                return default;
            }

            var secondsSince1970 = BinaryPrimitives.ReadUInt32LittleEndian(peHeaderPrefix[8..]);
            return secondsSince1970 == 0
                ? default
                : DateTimeOffset.FromUnixTimeSeconds(secondsSince1970).LocalDateTime;
        }
        catch
        {
            return default;
        }
    }

    private static DateTime? GetFileModificationTime(string exePath)
    {
        try
        {
            var fileInfo = new FileInfo(exePath);
            return fileInfo.LastWriteTime;
        }
        catch
        {
            return default;
        }
    }
}
