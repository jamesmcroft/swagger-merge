using System.Text.Json;
using System.Text.Json.Serialization;

namespace SwaggerMerge.Document;

/// <summary>
/// Defines the configuration for the Swagger document JSON files.
/// </summary>
public class SwaggerDocumentJson
{
    /// <summary>
    /// Gets or sets the <see cref="JsonSerializerOptions"/> that are required for serializing and deserializing Swagger documents.
    /// </summary>
    /// <remarks>
    /// The settings are used to ensure that the loaded and saved JSON files are in the original, correct format.
    /// </remarks>
    public static readonly JsonSerializerOptions Settings = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Always,
    };
}
