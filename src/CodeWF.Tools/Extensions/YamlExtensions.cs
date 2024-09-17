using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeWF.Tools.Extensions;

public static class YamlExtensions
{
    public static bool ToYaml<T>(this T obj, out string? yaml, out string? errorMsg)
    {
        if (obj == null)
        {
            yaml = default;
            errorMsg = "Please provide object";
            return false;
        }

        try
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithIndentedSequences()
                .Build();

            yaml = serializer.Serialize(obj);
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            yaml = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool FromYaml<T>(this string? yaml, out T? obj, out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            obj = default;
            errorMsg = "Please provide yaml string";
            return false;
        }

        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            ;

            obj = deserializer.Deserialize<T>(yaml);
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            obj = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool FromYaml(this string? yaml, out object? obj, out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            obj = default;
            errorMsg = "Please provide yaml string";
            return false;
        }

        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            ;

            obj = deserializer.Deserialize(yaml!);
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            obj = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool YamlPrettify(this string? rawYamlSting, out string? newYamlString, out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(rawYamlSting))
        {
            newYamlString = default;
            errorMsg = "Please provide Yaml string";
            return false;
        }

        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithIndentedSequences()
                .Build();

            var obj = deserializer.Deserialize(rawYamlSting);
            newYamlString = serializer.Serialize(obj);
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            newYamlString = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool YamlToJson(this string? yamlString, out string? jsonString, out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(yamlString))
        {
            jsonString = default;
            errorMsg = "Please provide Yaml string";
            return false;
        }

        try
        {
            if (yamlString.FromYaml(out var newObj, out errorMsg)
                && newObj.ToJson(out jsonString, out errorMsg))
            {
                return true;
            }
            else
            {
                jsonString = default;
                return false;
            }
        }
        catch (Exception ex)
        {
            jsonString = default;
            errorMsg = ex.Message;
            return false;
        }
    }
}