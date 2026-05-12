namespace SwaggerMerge.V3.Document;

using SwaggerMerge.Common.Document;

using System.Text;

/// <summary>
/// Defines an implementation for handling <see cref="OpenApiDocument"/> objects.
/// Delegates serialization to format-specific <see cref="IOpenApiDocumentFormatHandler"/> implementations.
/// </summary>
public class OpenApiDocumentHandler : IOpenApiDocumentHandler
{
    private readonly IOpenApiDocumentFormatHandler _jsonHandler = new OpenApiJsonDocumentFormatHandler();
    private readonly IOpenApiDocumentFormatHandler _yamlHandler = new OpenApiYamlDocumentFormatHandler();

    /// <inheritdoc/>
    public async Task<OpenApiDocument> LoadFromFilePathAsync(string filePath)
    {
        var content = await ReadAllTextAsync(filePath);
        var format = IOpenApiDocumentHandler.DetectFormat(filePath, content);
        return GetHandler(format).Deserialize(content);
    }

    /// <inheritdoc/>
    public OpenApiDocument LoadFromJson(string json)
    {
        return _jsonHandler.Deserialize(json);
    }

    /// <inheritdoc/>
    public OpenApiDocument LoadFromYaml(string yaml)
    {
        return _yamlHandler.Deserialize(yaml);
    }

    /// <inheritdoc/>
    public Task SaveToPathAsync(OpenApiDocument document, string filePath)
    {
        var format = IOpenApiDocumentHandler.DetectFormat(filePath);
        return this.SaveToPathAsync(document, filePath, format);
    }

    /// <inheritdoc/>
    public async Task SaveToPathAsync(OpenApiDocument document, string filePath, DocumentFormat format)
    {
        var content = GetHandler(format).Serialize(document);
        await WriteAllTextAsync(content, filePath);
    }

    private IOpenApiDocumentFormatHandler GetHandler(DocumentFormat format) =>
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
