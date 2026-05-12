namespace SwaggerMerge.Document;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.Serialization;

/// <summary>
/// Defines an implementation for handling <see cref="SwaggerDocument"/> objects.
/// </summary>
public class SwaggerDocumentHandler : ISwaggerDocumentHandler
{
    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="filePath">The file path to the Swagger document.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the file.</returns>
    public async Task<SwaggerDocument> LoadFromFilePathAsync(string filePath)
    {
        var content = await ReadAllTextAsync(filePath);
        var format = ISwaggerDocumentHandler.DetectFormat(filePath, content);
        return format == DocumentFormat.Yaml ? this.LoadFromYaml(content) : this.LoadFromJson(content);
    }

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified JSON string representing the document.
    /// </summary>
    /// <param name="swaggerJson">The <see cref="string"/> representing the Swagger document JSON.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON content.</returns>
    public SwaggerDocument LoadFromJson(string swaggerJson)
    {
        var deserializedContent =
            JsonSerializer.Deserialize(swaggerJson, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument);
        return deserializedContent ?? throw new InvalidOperationException(
            "The Swagger document JSON could not be loaded correctly as the format is not as expected.");
    }

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified YAML string representing the document.
    /// </summary>
    /// <param name="swaggerYaml">The <see cref="string"/> representing the Swagger document YAML.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the YAML content.</returns>
    public SwaggerDocument LoadFromYaml(string swaggerYaml)
    {
        var json = ConvertYamlToJson(swaggerYaml);
        return this.LoadFromJson(json);
    }

    /// <summary>
    /// Saves the specified <see cref="SwaggerDocument"/> to the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to the location where the Swagger file should be saved.</param>
    /// <returns>An asynchronous operation.</returns>
    public Task SaveToPathAsync(SwaggerDocument document, string filePath)
    {
        var format = ISwaggerDocumentHandler.DetectFormat(filePath);
        return this.SaveToPathAsync(document, filePath, format);
    }

    /// <summary>
    /// Saves the specified <see cref="SwaggerDocument"/> to the specified path in the specified format.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to the location where the Swagger file should be saved.</param>
    /// <param name="format">The format to save the document in.</param>
    /// <returns>An asynchronous operation.</returns>
    public async Task SaveToPathAsync(SwaggerDocument document, string filePath, DocumentFormat format)
    {
        var json = JsonSerializer.Serialize(document, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument);

        if (format == DocumentFormat.Yaml)
        {
            var yaml = ConvertJsonToYaml(json);
            await WriteAllTextAsync(yaml, filePath);
        }
        else
        {
            await WriteAllTextAsync(json, filePath);
        }
    }

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "YamlDotNet reflection builders are used only as an intermediary YAML<->JSON converter. The actual document model uses AOT-compatible System.Text.Json source generation.")]
    private static string ConvertYamlToJson(string yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize<object>(yaml);
        var serializer = new SerializerBuilder()
            .JsonCompatible()
            .Build();
        return serializer.Serialize(yamlObject);
    }

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "YamlDotNet reflection builders are used only as an intermediary JSON->YAML converter. The actual document model uses AOT-compatible System.Text.Json source generation.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "JsonNode deserialization does not require type metadata.")]
    private static string ConvertJsonToYaml(string json)
    {
        var jsonNode = JsonNode.Parse(json);
        var nativeObject = ConvertJsonNodeToNative(jsonNode);
        var serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
        return serializer.Serialize(nativeObject!);
    }

    private static object? ConvertJsonNodeToNative(JsonNode? node)
    {
        return node switch
        {
            JsonObject obj => obj.ToDictionary(kvp => kvp.Key, kvp => ConvertJsonNodeToNative(kvp.Value)),
            JsonArray arr => arr.Select(ConvertJsonNodeToNative).ToList(),
            JsonValue val => ConvertJsonValueToNative(val),
            null => null,
            _ => node.ToString()
        };
    }

    private static object? ConvertJsonValueToNative(JsonValue value)
    {
        if (value.TryGetValue<bool>(out var boolVal)) return boolVal;
        if (value.TryGetValue<long>(out var longVal)) return longVal;
        if (value.TryGetValue<double>(out var doubleVal)) return doubleVal;
        if (value.TryGetValue<string>(out var strVal)) return strVal;
        return value.ToString();
    }

    private static async Task<string> ReadAllTextAsync(string filePath)
    {
        using var stream = new StreamReader(filePath, Encoding.UTF8);
        return await stream.ReadToEndAsync();
    }

    private static async Task WriteAllTextAsync(string content, string filePath)
    {
        await using var stream = new StreamWriter(filePath, false, Encoding.UTF8);
        await stream.WriteAsync(content);
    }
}
