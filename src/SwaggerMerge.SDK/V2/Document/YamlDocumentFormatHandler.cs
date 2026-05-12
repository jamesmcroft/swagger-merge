namespace SwaggerMerge.V2.Document;

using SwaggerMerge.Common.Document;

using System.Text.Json;
using YamlDotNet.Core;

/// <summary>
/// Handles serialization and deserialization of <see cref="SwaggerDocument"/> objects in YAML format.
/// Uses AOT-safe YamlDotNet representation model APIs, converting via JSON as an intermediary.
/// </summary>
public class YamlDocumentFormatHandler : IDocumentFormatHandler
{
    private readonly JsonDocumentFormatHandler _jsonHandler = new();

    /// <inheritdoc/>
    public DocumentFormat Format => DocumentFormat.Yaml;

    /// <inheritdoc/>
    public SwaggerDocument Deserialize(string content)
    {
        try
        {
            var json = YamlJsonConverter.ConvertYamlToJson(content);
            return _jsonHandler.Deserialize(json);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JsonException or YamlException)
        {
            throw new InvalidOperationException(
                "The Swagger document YAML could not be loaded correctly as the format is not as expected.", ex);
        }
    }

    /// <inheritdoc/>
    public string Serialize(SwaggerDocument document)
    {
        var json = _jsonHandler.Serialize(document);
        return YamlJsonConverter.ConvertJsonToYaml(json);
    }
}
