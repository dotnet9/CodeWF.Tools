using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CodeWF.Tools.Extensions;

public static class ThisAssembly
{
    public static string? Title { get; private set; }
    public static string? Description { get; private set; }
    public static string? Company { get; private set; }
    public static string? Product { get; private set; }
    public static string? Version { get; private set; }
    public static string? InformationalVersion { get; private set; }
    public static string? FileVersion { get; private set; }
    public static string? Copyright { get; private set; }
    public static DateTime? CompileTime { get; private set; }

    static ThisAssembly()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == default)
            {
                return;
            }

            var atts = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (atts.Length > 0)
            {
                Title = ((AssemblyTitleAttribute)atts[0]).Title;
            }

            atts = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (atts.Length > 0)
            {
                Description = ((AssemblyDescriptionAttribute)atts[0]).Description;
            }

            atts = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (atts.Length > 0)
            {
                Company = ((AssemblyCompanyAttribute)atts[0]).Company;
            }

            atts = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (atts.Length > 0)
            {
                Product = ((AssemblyProductAttribute)atts[0]).Product;
            }

            Version = assembly.GetName()?.Version?.ToString();

            atts = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (atts.Length > 0)
            {
                InformationalVersion = ((AssemblyInformationalVersionAttribute)atts[0]).InformationalVersion;
            }

            atts = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (atts.Length > 0)
            {
                FileVersion = ((AssemblyFileVersionAttribute)atts[0]).Version;
            }

            atts = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (atts.Length > 0)
            {
                Copyright = ((AssemblyCopyrightAttribute)atts[0]).Copyright;
            }

            var exePath = Process.GetCurrentProcess()?.MainModule?.FileName;
            if (!string.IsNullOrWhiteSpace(exePath) && File.Exists(exePath))
            {
                CompileTime = new FileInfo(exePath).LastWriteTime;
            }
        }
        catch
        {
            // ignored
        }
    }
}