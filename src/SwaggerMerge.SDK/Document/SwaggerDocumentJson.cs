namespace SwaggerMerge.Document;

using Newtonsoft.Json;

/// <summary>
/// Defines the configuration for the Swagger document JSON files.
/// </summary>
public class SwaggerDocumentJson
{
    /// <summary>
    /// Gets or sets the <see cref="JsonSerializerSettings"/> that are required for serializing and deserializing Swagger documents.
    /// </summary>
    /// <remarks>
    /// The settings are used to ensure that the loaded and saved JSON files are in the original, correct format.
    /// </remarks>
    public static readonly JsonSerializerSettings Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
    };
}
