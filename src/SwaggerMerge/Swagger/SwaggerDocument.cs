namespace SwaggerMerge.Swagger;

using Newtonsoft.Json;

/// <summary>
/// Defines the detail of a Swagger document.
/// </summary>
public class SwaggerDocument
{
    /// <summary>
    /// Gets or sets the version of the Swagger specification.
    /// </summary>
    [JsonProperty("swagger")]
    public string SwaggerVersion { get; set; } = "2.0";

    /// <summary>
    /// Gets or sets the Swagger document information.
    /// </summary>
    [JsonProperty("info")]
    public SwaggerDocumentInfo Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the paths available in the Swagger document.
    /// </summary>
    [JsonProperty("paths")]
    public Dictionary<string, object> Paths { get; set; } = new();

    /// <summary>
    /// Gets or sets the definitions available in the Swagger document.
    /// </summary>
    [JsonProperty("definitions")]
    public Dictionary<string, object> Definitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the security definitions available in the Swagger document.
    /// </summary>
    [JsonProperty("securityDefinitions")]
    public Dictionary<string, object> SecurityDefinitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the security options available in the Swagger document.
    /// </summary>
    [JsonProperty("security")]
    public List<object> Security { get; set; } = new();
}