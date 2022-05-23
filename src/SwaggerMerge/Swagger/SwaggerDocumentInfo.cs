namespace SwaggerMerge.Swagger;

using Newtonsoft.Json;

/// <summary>
/// Defines the detail of the information section of a Swagger document.
/// </summary>
public class SwaggerDocumentInfo
{
    /// <summary>
    /// Gets or sets the title of the Swagger document.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the API version of the Swagger document.
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; }
}