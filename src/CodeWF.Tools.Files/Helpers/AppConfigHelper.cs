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
}