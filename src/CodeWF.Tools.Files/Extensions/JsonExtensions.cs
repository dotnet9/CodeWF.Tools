using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using YamlDotNet.System.Text.Json;

namespace CodeWF.Tools.Extensions;
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
public static class JsonExtensions
{
    public static bool ToJson<T>(this T obj, out string? json, out string? errorMsg)
    {
        if (obj == null)
        {
            json = default;
            errorMsg = "Please provide object";
            return false;
        }

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };
        try
        {
            json = JsonSerializer.Serialize(obj, options);
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            json = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool FromJson<T>(this string? json, out T? obj, out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            obj = default;
            errorMsg = "Please provide json string";
            return false;
        }

        try
        {
            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Converters = { new NullableDateTimeConverter() }
            };
            obj = JsonSerializer.Deserialize<T>(json!, options);
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

    public static bool JsonPrettify(this string? rawJsonString, int indent, bool sortKeys, out string? newJsonString,
        out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(rawJsonString))
        {
            newJsonString = default;
            errorMsg = "Please provide Json string";
            return false;
        }

        try
        {
            var writerOptions = new JsonWriterOptions
            {
                Indented = indent > 0,
                IndentSize = indent,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            using var jsonDoc = JsonDocument.Parse(rawJsonString);
            var sortedElement =
                sortKeys ? SortJsonElement(jsonDoc.RootElement, writerOptions) : jsonDoc.RootElement;
            using var stream = new MemoryStream();


            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                sortedElement.WriteTo(writer);
            }

            newJsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            newJsonString = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    public static bool JsonToYaml(this string? jsonString, out string? yamlString, out string? errorMsg)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            yamlString = default;
            errorMsg = "Please provide Json string";
            return false;
        }

        try
        {
            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };
            yamlString = YamlConverter.SerializeJson(jsonString!, jsonSerializerOptions: options);
            errorMsg = default;
            return true;
        }
        catch (Exception ex)
        {
            yamlString = default;
            errorMsg = ex.Message;
            return false;
        }
    }

    private static JsonElement SortJsonElement(JsonElement element, JsonWriterOptions writerOptions)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var sortedObject = new SortedDictionary<string, JsonElement>();
                foreach (var property in element.EnumerateObject())
                {
                    sortedObject[property.Name] = SortJsonElement(property.Value, writerOptions);
                }

                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, writerOptions))
                    {
                        writer.WriteStartObject();
                        foreach (var kvp in sortedObject)
                        {
                            writer.WritePropertyName(kvp.Key);
                            kvp.Value.WriteTo(writer);
                        }

                        writer.WriteEndObject();
                    }

                    return JsonDocument.Parse(stream.ToArray()).RootElement;
                }

            case JsonValueKind.Array:
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, writerOptions))
                    {
                        writer.WriteStartArray();
                        foreach (var item in element.EnumerateArray())
                        {
                            var sortedItem = SortJsonElement(item, writerOptions);
                            sortedItem.WriteTo(writer);
                        }

                        writer.WriteEndArray();
                    }

                    return JsonDocument.Parse(stream.ToArray()).RootElement;
                }

            case JsonValueKind.Undefined:
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            default:
                return element;
        }
    }
}