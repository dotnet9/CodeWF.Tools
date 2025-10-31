namespace CodeWF.Tools.Helpers;

using System;
using System.Configuration;

public static class AppConfigHelper
{
    public static Configuration OpenConfig(string filePath)
    {
        var fileMap = new ExeConfigurationFileMap
        {
            ExeConfigFilename = filePath
        };
        var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        return config;
    }

    public static bool TryGet<T>(this Configuration config, string name, out T? value)
    {
        try
        {
            var result = config.AppSettings.Settings[name].Value;
            if (typeof(T) == typeof(string))
            {
                value = (T)(object)result;
            }
            else
            {
                value = (T)Convert.ChangeType(result, typeof(T));
            }

            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public static bool TryGet<T>(string name, out T? value)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        return config.TryGet(name, out value);
    }

    public static bool TryGetOrSet<T>(this Configuration config, string name, out T? value, T? defaultValue = default)
    {
        try
        {
            if (config.TryGet(name, out value))
            {
                return true;
            }
            else
            {
                config.SetOrAdd(name, defaultValue);
                value = defaultValue;
                return false;
            }
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public static bool TryGetOrSet<T>(string name, out T? value, T? defaultValue = default)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        return config.TryGetOrSet(name, out value, defaultValue);
    }

    public static void Set<T>(this Configuration config, string name, T value)
    {
        config.AppSettings.Settings[name].Value = value?.ToString();
        config.Save(ConfigurationSaveMode.Modified);
    }

    public static void Set<T>(string name, T value)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.Set(name, value);
    }

    public static void SetOrAdd<T>(this Configuration config, string name, T value)
    {
        try
        {
            config.AppSettings.Settings[name].Value = value?.ToString();
        }
        catch
        {
            config.AppSettings.Settings.Add(name, value?.ToString());
        }

        config.Save(ConfigurationSaveMode.Modified);
    }

    public static void SetOrAdd<T>(string name, T value)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); 
        config.SetOrAdd(name, value);
    }
    public static T GetOrAdd<T>(this Configuration config, string name, T? defaultValue)
    {
        var value = defaultValue;
        config.TryGetOrSet(name, out value, defaultValue);
        return value;
    }

    public static T GetOrAdd<T>(string name, T? defaultValue)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        return config.GetOrAdd(name, defaultValue);
    }
}