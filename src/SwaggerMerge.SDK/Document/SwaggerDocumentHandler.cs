namespace SwaggerMerge.Document;

using System.Text;
using Newtonsoft.Json;

/// <summary>
/// Defines an implementation for handling <see cref="SwaggerDocument"/> objects.
/// </summary>
public class SwaggerDocumentHandler : ISwaggerDocumentHandler
{
    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified path.
    /// </summary>
    /// <param name="filePath">The file path to the Swagger JSON document.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON file.</returns>
    public async Task<SwaggerDocument> LoadFromFilePathAsync(string filePath)
    {
        var swaggerJson = await ReadAllTextAsync(filePath);
        return this.LoadFromJson(swaggerJson);
    }

    /// <summary>
    /// Loads a <see cref="SwaggerDocument"/> from the specified JSON string representing the document.
    /// </summary>
    /// <param name="swaggerJson">The <see cref="string"/> representing the Swagger document JSON.</param>
    /// <returns>A <see cref="SwaggerDocument"/> representing the JSON content.</returns>
    public SwaggerDocument LoadFromJson(string swaggerJson)
    {
        var deserializedContent = JsonConvert.DeserializeObject<SwaggerDocument>(swaggerJson, SwaggerDocumentJson.Settings);
        if (deserializedContent == null)
        {
            throw new InvalidOperationException(
                "The Swagger document JSON could not be loaded correctly as the format is not as expected.");
        }

        return deserializedContent;
    }

    /// <summary>
    /// Saves the specified <see cref="SwaggerDocument"/> to the specified path.
    /// </summary>
    /// <param name="document">The document to save.</param>
    /// <param name="filePath">The file path to the location where the Swagger JSON file should be saved.</param>
    /// <returns>An asynchronous operation.</returns>
    public async Task SaveToPathAsync(SwaggerDocument document, string filePath)
    {
        var swaggerJson = JsonConvert.SerializeObject(document, SwaggerDocumentJson.Settings);
        await WriteAllTextAsync(swaggerJson, filePath);
    }

    private static async Task<string> ReadAllTextAsync(string filePath)
    {
        using var stream = new StreamReader(filePath, Encoding.UTF8);
        return await stream.ReadToEndAsync();
    }

    private static async Task WriteAllTextAsync(string content, string filePath)
    {
        using var stream = new StreamWriter(filePath, false, Encoding.UTF8);
        await stream.WriteAsync(content);
    }
}