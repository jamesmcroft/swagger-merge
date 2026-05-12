namespace SwaggerMerge.V2.Document;

using SwaggerMerge.Common.Document;

/// <summary>
/// Defines an interface for serializing and deserializing <see cref="SwaggerDocument"/> objects in a specific format.
/// </summary>
public interface IDocumentFormatHandler
{
    /// <summary>
    /// Gets the document format this handler supports.
    /// </summary>
    DocumentFormat Format { get; }

    /// <summary>
    /// Deserializes a <see cref="SwaggerDocument"/> from the specified content string.
    /// </summary>
    /// <param name="content">The string content representing the Swagger document.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the content.</returns>
    SwaggerDocument Deserialize(string content);

    /// <summary>
    /// Serializes a <see cref="SwaggerDocument"/> to a string in this handler's format.
    /// </summary>
    /// <param name="document">The document to serialize.</param>
    /// <returns>A string representation of the document.</returns>
    string Serialize(SwaggerDocument document);
}
