namespace CodeWF.Tools.Helpers;

using System;
using System.Configuration;

public class AppConfigHelper
{
    public static bool TryGet<T>(string name, out T? value)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var result = config.AppSettings.Settings[name].Value;
        try
        {
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

    public static void Set<T>(string name, T value)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings[name].Value = value?.ToString();
        config.Save(ConfigurationSaveMode.Modified);
    }
}