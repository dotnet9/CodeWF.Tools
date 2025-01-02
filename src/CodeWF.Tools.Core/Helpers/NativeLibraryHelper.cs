using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CodeWF.Tools.Helpers;

/// <summary>
/// NativeLibrary帮助类
/// </summary>
public static class NativeLibraryHelper
{
    // VC库句柄，Key：库名，Value：库句柄
    private static Dictionary<string, nint?> _libraryIntPtrs = new();

    // VC库方法句饼，Key：库名+方法名，Value：方法句柄
    private static Dictionary<string, nint?> _libraryMethodIntPtrs = new();

    public static bool LoadLibrary(string dllDir, string dllNamePrefix, string x64Suffix, string x86Suffix,
        out string? errorMessage)
    {
        return IsLoadLibrary(dllDir, dllNamePrefix, x64Suffix, x86Suffix, out errorMessage);
    }

    public static bool GetMethod(string dllDir, string dllNamePrefix, string x64Suffix, string x86Suffix,
        string methodName, out nint? ptr, out string? errorMsg)
    {
        ptr = default;

        if (!IsLoadMethod(dllDir, dllNamePrefix, x64Suffix, x86Suffix, methodName, out ptr, out errorMsg))
        {
            return false;
        }

        return true;
    }

    public static bool GetMethod(string methodName, out nint? ptr, out string? errorMsg)
    {
        ptr = default;
        if (!GetDefaultLibrary(out var dllPtr, out errorMsg))
        {
            return false;
        }

        return IsLoadMethod(dllPtr.Value, methodName, out ptr, out errorMsg);
    }

    private static bool TryGetLibraryPath(string dllDir, string dllNamePrefix, string x64Suffix, string x86Suffix,
        out string? dllPath, out string? errorMsg)
    {
        dllPath = default;
        errorMsg = default;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var bitField = Environment.Is64BitProcess ? x64Suffix : x86Suffix;
            dllPath = Path.Combine(dllDir, $"{dllNamePrefix}{bitField}.dll");
            return true;
        }
        else
        {
            errorMsg = "Non Windows platforms to be supported";
            return false;
        }
    }

    private static bool GetDefaultLibrary(out IntPtr? dllPtr, out string? errorMsg)
    {
        dllPtr = default;
        errorMsg = default;
        if (!_libraryIntPtrs.Any())
        {
            errorMsg = "Please load the dynamic library first";
            return false;
        }

        dllPtr = _libraryIntPtrs.FirstOrDefault().Value;
        if (dllPtr.HasValue && dllPtr.Value != IntPtr.Zero)
        {
            return true;
        }

        errorMsg = "Please load the dynamic library first";
        return false;
    }

    private static bool IsLoadLibrary(string dllDir, string dllNamePrefix, string x64Suffix, string x86Suffix,
        out string? errorMsg)
    {
        if (!TryGetLibraryPath(dllDir, dllNamePrefix, x64Suffix, x86Suffix, out var dllPath, out errorMsg))
        {
            return false;
        }

        if (!File.Exists(dllPath))
        {
            errorMsg = $"Please configure the dynamic library [{dllPath}]";
            return false;
        }

        if (_libraryIntPtrs.ContainsKey(dllPath))
        {
            return true;
        }

        var dllPtr = NativeLibrary.Load(dllPath);
        if (dllPtr == IntPtr.Zero)
        {
            errorMsg = $"Dynamic library loading failed, please check: [{dllPath}]";
            return false;
        }

        _libraryIntPtrs[dllPath] = dllPtr;
        return true;
    }

    private static bool IsLoadMethod(string dllDir, string dllNamePrefix, string x64Suffix, string x86Suffix,
        string methodName, out IntPtr? methodPtr, out string? errorMsg)
    {
        methodPtr = default;
        if (!IsLoadLibrary(dllDir, dllNamePrefix, x64Suffix, x86Suffix, out errorMsg))
        {
            return false;
        }

        TryGetLibraryPath(dllDir, dllNamePrefix, x64Suffix, x86Suffix, out var dllPath, out errorMsg);
        var methodKey = $"{dllPath}+{methodName}";
        if (_libraryMethodIntPtrs.ContainsKey(methodKey))
        {
            return true;
        }

        var dllPtr = _libraryIntPtrs[dllPath];
        return IsLoadMethod(dllPtr, methodName, out methodPtr, out errorMsg);
    }

    private static bool IsLoadMethod(IntPtr? dllPtr, string methodName, out IntPtr? methodPtr, out string? errorMsg)
    {
        methodPtr = default;
        errorMsg = default;
        if (!dllPtr.HasValue || dllPtr == IntPtr.Zero)
        {
            errorMsg = $"Dynamic library loading failed, please check";
            return false;
        }

        methodPtr = NativeLibrary.GetExport(dllPtr.Value, methodName);

        if (methodPtr != IntPtr.Zero)
        {
            return true;
        }

        errorMsg =
            $"The dynamic library does not provide method export, method name is [{methodName}]";
        return false;
    }
}