namespace SwaggerMerge.V3.Document;

using SwaggerMerge.Common.Document;

/// <summary>
/// Defines an interface for serializing and deserializing <see cref="OpenApiDocument"/> objects in a specific format.
/// </summary>
public interface IOpenApiDocumentFormatHandler
{
    /// <summary>
    /// Gets the document format this handler supports.
    /// </summary>
    DocumentFormat Format { get; }

    /// <summary>
    /// Deserializes an <see cref="OpenApiDocument"/> from the specified content string.
    /// </summary>
    /// <param name="content">The string content representing the OpenAPI document.</param>
    /// <returns>An <see cref="OpenApiDocument"/> representing the content.</returns>
    OpenApiDocument Deserialize(string content);

    /// <summary>
    /// Serializes an <see cref="OpenApiDocument"/> to a string in this handler's format.
    /// </summary>
    /// <param name="document">The document to serialize.</param>
    /// <returns>A string representation of the document.</returns>
    string Serialize(OpenApiDocument document);
}
