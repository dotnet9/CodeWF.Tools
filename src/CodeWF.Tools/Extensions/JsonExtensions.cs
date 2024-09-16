using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CodeWF.Tools.Extensions;

public static class JsonExtensions
{
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