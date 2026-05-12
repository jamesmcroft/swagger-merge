namespace SwaggerMerge.Document.V3;

using System.Text.Json;

/// <summary>
/// Handles serialization and deserialization of <see cref="OpenApiDocument"/> objects in JSON format.
/// </summary>
public class OpenApiJsonDocumentFormatHandler : IOpenApiDocumentFormatHandler
{
    /// <inheritdoc/>
    public DocumentFormat Format => DocumentFormat.Json;

    /// <inheritdoc/>
    public OpenApiDocument Deserialize(string content)
    {
        var deserializedContent =
            JsonSerializer.Deserialize(content, OpenApiDocumentJsonSerializerContext.Default.OpenApiDocument);
        return deserializedContent ?? throw new InvalidOperationException(
            "The OpenAPI document JSON could not be loaded correctly as the format is not as expected.");
    }

    /// <inheritdoc/>
    public string Serialize(OpenApiDocument document)
    {
        return JsonSerializer.Serialize(document, OpenApiDocumentJsonSerializerContext.Default.OpenApiDocument);
    }
}
