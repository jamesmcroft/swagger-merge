namespace SwaggerMerge.V2.Document;

using SwaggerMerge.Common.Document;

using System.Text;

/// <summary>
/// Defines an implementation for handling <see cref="SwaggerDocument"/> objects.
/// Delegates serialization to format-specific <see cref="IDocumentFormatHandler"/> implementations.
/// </summary>
public class SwaggerDocumentHandler : ISwaggerDocumentHandler
{
    private readonly IDocumentFormatHandler _jsonHandler = new JsonDocumentFormatHandler();
    private readonly IDocumentFormatHandler _yamlHandler = new YamlDocumentFormatHandler();

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified path, auto-detecting format from the file extension.
    /// </summary>
    /// <param name="filePath">The file path to the Swagger document.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the file.</returns>
    public async Task<SwaggerDocument> LoadFromFilePathAsync(string filePath)
    {
        var content = await ReadAllTextAsync(filePath);
        var format = ISwaggerDocumentHandler.DetectFormat(filePath, content);
        return GetHandler(format).Deserialize(content);
    }

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified JSON string representing the document.
    /// </summary>
    /// <param name="swaggerJson">The <see cref="string"/> representing the Swagger document JSON.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON content.</returns>
    public SwaggerDocument LoadFromJson(string swaggerJson)
    {
        return _jsonHandler.Deserialize(swaggerJson);
    }

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified YAML string representing the document.
    /// </summary>
    /// <param name="swaggerYaml">The <see cref="string"/> representing the Swagger document YAML.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the YAML content.</returns>
    public SwaggerDocument LoadFromYaml(string swaggerYaml)
    {
        return _yamlHandler.Deserialize(swaggerYaml);
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
        var content = GetHandler(format).Serialize(document);
        await WriteAllTextAsync(content, filePath);
    }

    private IDocumentFormatHandler GetHandler(DocumentFormat format) =>
        format == DocumentFormat.Yaml ? _yamlHandler : _jsonHandler;

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
