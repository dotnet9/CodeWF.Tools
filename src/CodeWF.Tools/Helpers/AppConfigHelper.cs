using CodeWF.Tools.Extensions;
using System.Reflection;

namespace CodeWF.Tools.Helpers;

using System;
using System.Configuration;
using System.IO;

public class AppConfigHelper
{
    private readonly Configuration _config;

    private AppConfigHelper(string configPath)
    {
        var config = ConfigurationManager.OpenExeConfiguration(configPath);
        _config = config;
    }

    public static string GetEntryAssemblyConfigPath()
    {
        var appConfigPath = Assembly.GetEntryAssembly()?.Location;
        return $"{appConfigPath}.config";
    }

    public static string GetExecutingAssemblyConfigPath()
    {
        var appConfigPath = Assembly.GetExecutingAssembly().Location;
        return $"{appConfigPath}.config";
    }

    public static string GetCallingAssemblyConfigPath()
    {
        var appConfigPath = Assembly.GetCallingAssembly().Location;
        return $"{appConfigPath}.config";
    }

    public static AppConfigHelper GetEntryAssembly()
    {
        return GetCustom(Assembly.GetEntryAssembly()!.Location);
    }

    public static AppConfigHelper GetExecutingAssembly()
    {
        return GetCustom(Assembly.GetExecutingAssembly()!.Location);
    }

    public static AppConfigHelper GetCallingAssembly()
    {
        return GetCustom(Assembly.GetCallingAssembly()!.Location);
    }

    public static AppConfigHelper GetCustom(string configFilePath)
    {
        if (configFilePath.IsNullOrEmpty() || !File.Exists(configFilePath))
        {
            throw new FileNotFoundException($"Config file not found: {configFilePath}");
        }

        return new AppConfigHelper(configFilePath);
    }


    public void Set<T>(string name, T value)
    {
        _config.AppSettings.Settings[name].Value = value!.ToString();
        _config.Save(ConfigurationSaveMode.Modified);
    }

    public T Get<T>(string name)
    {
        var value = _config.AppSettings.Settings[name].Value;
        return (T)Convert.ChangeType(value, typeof(T));
    }

    public bool TryGet<T>(string name, out T value) where T : struct
    {
        var result = _config.AppSettings.Settings[name].Value;
        try
        {
            value = (T)Convert.ChangeType(result, typeof(T));
            return true;
        }
        catch
        {
            value = default(T);
            return false;
        }
    }
}