using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeWF.Tools.Extensions;

public static class YamlExtensions
{
    [RequiresDynamicCode("YamlDotNet reflection-based SerializerBuilder is not Native AOT safe. Use YamlDotNet static source-generated serializers for Native AOT.")]
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

    [RequiresDynamicCode("YamlDotNet reflection-based DeserializerBuilder is not Native AOT safe. Use YamlDotNet static source-generated deserializers for Native AOT.")]
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

    [RequiresDynamicCode("YamlDotNet reflection-based DeserializerBuilder is not Native AOT safe. Use YamlDotNet static source-generated deserializers for Native AOT.")]
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

    [RequiresDynamicCode("YamlDotNet reflection-based serializers are not Native AOT safe. Use YamlDotNet static source-generated serializers for Native AOT.")]
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
            using var reader = new StringReader(yamlString);
            var yaml = new YamlStream();
            yaml.Load(reader);

            using var stream = new MemoryStream();
            var writerOptions = new JsonWriterOptions
            {
                Indented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                if (yaml.Documents.Count == 0 || yaml.Documents[0].RootNode is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    WriteYamlNodeAsJson(writer, yaml.Documents[0].RootNode);
                }
            }

            jsonString = Encoding.UTF8.GetString(stream.ToArray());
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            jsonString = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    private static void WriteYamlNodeAsJson(Utf8JsonWriter writer, YamlNode node)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                writer.WriteStartObject();
                foreach (var child in mapping.Children)
                {
                    writer.WritePropertyName(GetMappingKey(child.Key));
                    WriteYamlNodeAsJson(writer, child.Value);
                }

                writer.WriteEndObject();
                break;

            case YamlSequenceNode sequence:
                writer.WriteStartArray();
                foreach (var child in sequence.Children)
                {
                    WriteYamlNodeAsJson(writer, child);
                }

                writer.WriteEndArray();
                break;

            case YamlScalarNode scalar:
                WriteYamlScalarAsJson(writer, scalar);
                break;

            default:
                writer.WriteNullValue();
                break;
        }
    }

    private static string GetMappingKey(YamlNode key)
    {
        return key is YamlScalarNode scalar ? scalar.Value ?? string.Empty : key.ToString() ?? string.Empty;
    }

    private static void WriteYamlScalarAsJson(Utf8JsonWriter writer, YamlScalarNode scalar)
    {
        var value = scalar.Value;
        if (scalar.Style != ScalarStyle.Plain)
        {
            writer.WriteStringValue(value);
            return;
        }

        if (value is null || value.Equals("null", StringComparison.OrdinalIgnoreCase) || value == "~")
        {
            writer.WriteNullValue();
            return;
        }

        if (bool.TryParse(value, out var boolean))
        {
            writer.WriteBooleanValue(boolean);
            return;
        }

        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            writer.WriteNumberValue(integer);
            return;
        }

        if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            writer.WriteNumberValue(number);
            return;
        }

        writer.WriteStringValue(value);
    }
}
