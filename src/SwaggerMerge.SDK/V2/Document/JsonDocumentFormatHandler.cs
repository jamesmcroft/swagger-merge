namespace SwaggerMerge.V2.Document;

using SwaggerMerge.Common.Document;

using System.Text.Json;

/// <summary>
/// Handles serialization and deserialization of <see cref="SwaggerDocument"/> objects in JSON format.
/// </summary>
public class JsonDocumentFormatHandler : IDocumentFormatHandler
{
    /// <inheritdoc/>
    public DocumentFormat Format => DocumentFormat.Json;

    /// <inheritdoc/>
    public SwaggerDocument Deserialize(string content)
    {
        var deserializedContent =
            JsonSerializer.Deserialize(content, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument);
        return deserializedContent ?? throw new InvalidOperationException(
            "The Swagger document JSON could not be loaded correctly as the format is not as expected.");
    }

    /// <inheritdoc/>
    public string Serialize(SwaggerDocument document)
    {
        return JsonSerializer.Serialize(document, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument);
    }
}
