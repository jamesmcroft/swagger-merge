namespace SwaggerMerge.V3.Document;

using SwaggerMerge.Common.Document;

/// <summary>
/// Defines an interface for handling <see cref="OpenApiDocument"/> objects.
/// </summary>
public interface IOpenApiDocumentHandler
{
    /// <summary>
    /// Loads an <see cref="OpenApiDocument"/> from the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="filePath">The file path to the OpenAPI document.</param>
    /// <returns>An <see cref="OpenApiDocument"/> representing the file.</returns>
    Task<OpenApiDocument> LoadFromFilePathAsync(string filePath);

    /// <summary>
    /// Loads an <see cref="OpenApiDocument"/> from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string representing the OpenAPI document.</param>
    /// <returns>An <see cref="OpenApiDocument"/> representing the JSON content.</returns>
    OpenApiDocument LoadFromJson(string json);

    /// <summary>
    /// Loads an <see cref="OpenApiDocument"/> from the specified YAML string.
    /// </summary>
    /// <param name="yaml">The YAML string representing the OpenAPI document.</param>
    /// <returns>An <see cref="OpenApiDocument"/> representing the YAML content.</returns>
    OpenApiDocument LoadFromYaml(string yaml);

    /// <summary>
    /// Saves the specified <see cref="OpenApiDocument"/> to the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to save to.</param>
    /// <returns>An asynchronous operation.</returns>
    Task SaveToPathAsync(OpenApiDocument document, string filePath);

    /// <summary>
    /// Saves the specified <see cref="OpenApiDocument"/> to the specified path in the specified format.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to save to.</param>
    /// <param name="format">The format to save the document in.</param>
    /// <returns>An asynchronous operation.</returns>
    Task SaveToPathAsync(OpenApiDocument document, string filePath, DocumentFormat format);

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
            _ when !string.IsNullOrWhiteSpace(content) => SniffContentFormat(content),
            _ => DocumentFormat.Json
        };
    }

    private static DocumentFormat SniffContentFormat(string content)
    {
        var trimmed = content.TrimStart();
        if (trimmed.Length == 0)
        {
            return DocumentFormat.Json;
        }

        if (trimmed[0] is '{' or '[')
        {
            try
            {
                System.Text.Json.JsonDocument.Parse(content).Dispose();
                return DocumentFormat.Json;
            }
            catch (System.Text.Json.JsonException)
            {
                return DocumentFormat.Yaml;
            }
        }

        return DocumentFormat.Yaml;
    }
}
