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
        string? exePath;
        
        // 尝试获取可执行文件路径
        try
        {
            exePath = Process.GetCurrentProcess()?.MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                // 如果获取失败，尝试使用 Assembly.Location
                exePath = assembly?.Location;
                if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                {
                    return default;
                }
            }
        }
        catch
        {
            return default;
        }

        // 检测当前操作系统平台
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // Windows 平台：读取 PE 头部信息
            return GetWindowsCompileTime(exePath);
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Linux 平台：读取 ELF 文件信息
            return GetLinuxCompileTime(exePath);
        }
        
        return default;
    }

    private static DateTime? GetWindowsCompileTime(string exePath)
    {
        try
        {
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
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            return dt.ToLocalTime();
        }
        catch
        {
            // 如果读取失败，返回默认值
            return default;
        }
    }

    private static DateTime? GetLinuxCompileTime(string exePath)
    {
        try
        {
            // 首先尝试使用 ELF 文件头获取时间戳
            var elfTime = GetElfTimestamp(exePath);
            if (elfTime.HasValue)
            {
                // 验证时间戳是否合理（不是1970年）
                if (elfTime.Value.Year > 1970)
                {
                    return elfTime;
                }
            }

            // 如果 ELF 方法失败，尝试使用文件修改时间作为备选方案
            return GetFileModificationTime(exePath);
        }
        catch
        {
            // 如果读取失败，返回默认值
            return default;
        }
    }

    private static DateTime? GetElfTimestamp(string exePath)
    {
        try
        {
            const int ElfHeaderSize = 64;
            var buffer = new byte[ElfHeaderSize];
            using (var s = new FileStream(exePath, FileMode.Open, FileAccess.Read))
            {
                // 读取整个 ELF 头部
                s.Read(buffer, 0, ElfHeaderSize);
            }

            // 验证 ELF 魔数
            if (buffer[0] != 0x7F || buffer[1] != 0x45 || buffer[2] != 0x4C || buffer[3] != 0x46)
            {
                // 不是有效的 ELF 文件
                return default;
            }

            // 读取时间戳（从文件开始偏移 8 字节处）
            var secondsSince1970 = BitConverter.ToInt32(buffer, 8);
            
            // 检查是否为大端字节序
            if (BitConverter.IsLittleEndian)
            {
                // 转换为小端字节序
                secondsSince1970 = System.Net.IPAddress.NetworkToHostOrder(secondsSince1970);
            }
            
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            return dt.ToLocalTime();
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