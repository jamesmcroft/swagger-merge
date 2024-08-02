namespace SwaggerMerge.Document;

/// <summary>
/// Defines an interface for handling <see cref="SwaggerDocument"/> objects.
/// </summary>
public interface ISwaggerDocumentHandler
{
    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified path.
    /// </summary>
    /// <param name="filePath">The file path to the Swagger JSON document.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON file.</returns>
    Task<SwaggerDocument> LoadFromFilePathAsync(string filePath);

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified JSON string representing the document.
    /// </summary>
    /// <param name="swaggerJson">The <see cref="string"/> representing the Swagger document JSON.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON content.</returns>
    SwaggerDocument LoadFromJson(string swaggerJson);

    /// <summary>
    /// Saves the specified <see cref="SwaggerDocument"/> to the specified path.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to the location where the Swagger JSON file should be saved.</param>
    /// <returns>An asynchronous operation.</returns>
    Task SaveToPathAsync(SwaggerDocument document, string filePath);
}
