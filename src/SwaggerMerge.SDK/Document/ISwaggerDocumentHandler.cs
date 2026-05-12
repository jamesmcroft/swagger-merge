namespace SwaggerMerge.Document;

/// <summary>
/// Defines an interface for handling <see cref="SwaggerDocument"/> objects.
/// </summary>
public interface ISwaggerDocumentHandler
{
    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="filePath">The file path to the Swagger document.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the file.</returns>
    Task<SwaggerDocument> LoadFromFilePathAsync(string filePath);

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified JSON string representing the document.
    /// </summary>
    /// <param name="swaggerJson">The <see cref="string"/> representing the Swagger document JSON.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON content.</returns>
    SwaggerDocument LoadFromJson(string swaggerJson);

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified YAML string representing the document.
    /// </summary>
    /// <param name="swaggerYaml">The <see cref="string"/> representing the Swagger document YAML.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the YAML content.</returns>
    SwaggerDocument LoadFromYaml(string swaggerYaml);

    /// <summary>
    /// Saves the specified <see cref="SwaggerDocument"/> to the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to the location where the Swagger file should be saved.</param>
    /// <returns>An asynchronous operation.</returns>
    Task SaveToPathAsync(SwaggerDocument document, string filePath);

    /// <summary>
    /// Saves the specified <see cref="SwaggerDocument"/> to the specified path in the specified format.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to the location where the Swagger file should be saved.</param>
    /// <param name="format">The format to save the document in.</param>
    /// <returns>An asynchronous operation.</returns>
    Task SaveToPathAsync(SwaggerDocument document, string filePath, DocumentFormat format);

    /// <summary>
    /// Detects the document format from the file extension, falling back to content sniffing.
    /// </summary>
    /// <param name="filePath">The file path to detect format from.</param>
    /// <param name="content">Optional content to sniff when the extension is ambiguous.</param>
    /// <returns>The detected <see cref="DocumentFormat"/>.</returns>
    static DocumentFormat DetectFormat(string filePath, string? content = null)
    {
        var extension = Path.GetExtension(filePath)?.ToLowerInvariant();
        return extension switch
        {
            ".yaml" or ".yml" => DocumentFormat.Yaml,
            ".json" => DocumentFormat.Json,
            _ => content != null
                ? (content.TrimStart().StartsWith('{') ? DocumentFormat.Json : DocumentFormat.Yaml)
                : DocumentFormat.Json
        };
    }
}
