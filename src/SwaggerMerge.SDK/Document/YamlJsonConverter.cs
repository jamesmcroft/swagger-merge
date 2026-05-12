namespace SwaggerMerge.Document;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

/// <summary>
/// Provides AOT-safe YAML-to-JSON and JSON-to-YAML conversion utilities shared by both V2 and V3 format handlers.
/// </summary>
internal static class YamlJsonConverter
{
    /// <summary>
    /// Converts a YAML string to a JSON string.
    /// </summary>
    /// <param name="yaml">The YAML content.</param>
    /// <returns>The equivalent JSON content.</returns>
    public static string ConvertYamlToJson(string yaml)
    {
        using var reader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        if (yamlStream.Documents.Count == 0)
        {
            return "{}";
        }

        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true });
        WriteYamlNodeAsJson(writer, yamlStream.Documents[0].RootNode);
        writer.Flush();
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    /// <summary>
    /// Converts a JSON string to a YAML string.
    /// </summary>
    /// <param name="json">The JSON content.</param>
    /// <returns>The equivalent YAML content.</returns>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "JsonNode deserialization does not require type metadata.")]
    public static string ConvertJsonToYaml(string json)
    {
        var jsonNode = JsonNode.Parse(json);
        var yamlNode = ConvertJsonNodeToYamlNode(jsonNode);

        var document = new YamlDocument(yamlNode);
        var yamlStream = new YamlStream(document);

        using var writer = new StringWriter();
        yamlStream.Save(writer, assignAnchors: false);
        var yaml = writer.ToString();

        // YamlStream.Save appends "..." document end marker; remove it for cleaner output
        const string documentEndMarker = "...\r\n";
        const string documentEndMarkerUnix = "...\n";
        if (yaml.EndsWith(documentEndMarker, StringComparison.Ordinal))
        {
            yaml = yaml[..^documentEndMarker.Length];
        }
        else if (yaml.EndsWith(documentEndMarkerUnix, StringComparison.Ordinal))
        {
            yaml = yaml[..^documentEndMarkerUnix.Length];
        }

        return yaml;
    }

    private static void WriteYamlNodeAsJson(Utf8JsonWriter writer, YamlNode node)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                writer.WriteStartObject();
                foreach (var entry in mapping.Children)
                {
                    var key = entry.Key is YamlScalarNode scalarKey
                        ? scalarKey.Value ?? string.Empty
                        : entry.Key.ToString();
                    writer.WritePropertyName(key);
                    WriteYamlNodeAsJson(writer, entry.Value);
                }
                writer.WriteEndObject();
                break;

            case YamlSequenceNode sequence:
                writer.WriteStartArray();
                foreach (var item in sequence.Children)
                {
                    WriteYamlNodeAsJson(writer, item);
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

    private static void WriteYamlScalarAsJson(Utf8JsonWriter writer, YamlScalarNode scalar)
    {
        var value = scalar.Value;

        if (value == null || value == "~" || value.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            writer.WriteNullValue();
            return;
        }

        // Quoted strings are always strings
        if (scalar.Style is ScalarStyle.SingleQuoted or ScalarStyle.DoubleQuoted)
        {
            writer.WriteStringValue(value);
            return;
        }

        // Only coerce unquoted scalars that are unambiguously typed.
        // Booleans: true/false (case-insensitive per YAML 1.1/1.2 core schema)
        if (bool.TryParse(value, out var boolVal))
        {
            writer.WriteBooleanValue(boolVal);
            return;
        }

        // Pure integers (no decimal point) are safe to coerce
        if (long.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var longVal)
            && !value.Contains('.'))
        {
            writer.WriteNumberValue(longVal);
            return;
        }

        // Treat all other scalars as strings to avoid coercing version-like
        // values (e.g. "2.0") to JSON numbers, which would break fields
        // defined as strings in the Swagger/OpenAPI specification.
        writer.WriteStringValue(value);
    }

    private static YamlNode ConvertJsonNodeToYamlNode(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
                var mapping = new YamlMappingNode();
                foreach (var kvp in obj)
                {
                    mapping.Add(new YamlScalarNode(kvp.Key), ConvertJsonNodeToYamlNode(kvp.Value));
                }
                return mapping;

            case JsonArray arr:
                var sequence = new YamlSequenceNode();
                foreach (var item in arr)
                {
                    sequence.Add(ConvertJsonNodeToYamlNode(item));
                }
                return sequence;

            case JsonValue val:
                return ConvertJsonValueToYamlScalar(val);

            default:
                return new YamlScalarNode("null") { Style = ScalarStyle.Plain };
        }
    }

    private static YamlScalarNode ConvertJsonValueToYamlScalar(JsonValue value)
    {
        if (value.TryGetValue<bool>(out var boolVal))
            return new YamlScalarNode(boolVal ? "true" : "false") { Style = ScalarStyle.Plain };

        if (value.TryGetValue<long>(out var longVal))
            return new YamlScalarNode(longVal.ToString(System.Globalization.CultureInfo.InvariantCulture)) { Style = ScalarStyle.Plain };

        if (value.TryGetValue<double>(out var doubleVal))
            return new YamlScalarNode(doubleVal.ToString(System.Globalization.CultureInfo.InvariantCulture)) { Style = ScalarStyle.Plain };

        if (value.TryGetValue<string>(out var strVal))
            return new YamlScalarNode(strVal) { Style = ScalarStyle.DoubleQuoted };

        return new YamlScalarNode(value.ToString()) { Style = ScalarStyle.DoubleQuoted };
    }
}
