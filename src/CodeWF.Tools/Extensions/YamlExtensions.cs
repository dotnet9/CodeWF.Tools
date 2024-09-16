using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeWF.Tools.Extensions;

public static class YamlExtensions
{
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
}