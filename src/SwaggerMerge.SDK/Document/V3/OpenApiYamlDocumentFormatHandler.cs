namespace SwaggerMerge.Document.V3;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

/// <summary>
/// Handles serialization and deserialization of <see cref="OpenApiDocument"/> objects in YAML format.
/// Uses AOT-safe YamlDotNet representation model APIs, converting via JSON as an intermediary.
/// </summary>
public class OpenApiYamlDocumentFormatHandler : IOpenApiDocumentFormatHandler
{
    private readonly OpenApiJsonDocumentFormatHandler _jsonHandler = new();

    /// <inheritdoc/>
    public DocumentFormat Format => DocumentFormat.Yaml;

    /// <inheritdoc/>
    public OpenApiDocument Deserialize(string content)
    {
        try
        {
            var json = YamlJsonConverter.ConvertYamlToJson(content);
            return _jsonHandler.Deserialize(json);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JsonException or YamlException)
        {
            throw new InvalidOperationException(
                "The OpenAPI document YAML could not be loaded correctly as the format is not as expected.", ex);
        }
    }

    /// <inheritdoc/>
    public string Serialize(OpenApiDocument document)
    {
        var json = _jsonHandler.Serialize(document);
        return YamlJsonConverter.ConvertJsonToYaml(json);
    }
}
