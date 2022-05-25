namespace SwaggerMerge.Merge.Configuration.Output;

using Swagger;

/// <summary>
/// Defines the configuration for a Swagger document output.
/// </summary>
public class SwaggerOutputConfiguration
{
    /// <summary>
    /// Gets or sets the file path of the output merged Swagger document.
    /// </summary>
    public string File { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the host (name or IP) service the API for the output.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base path on which the API is served relative to the <see cref="Host"/>.
    /// </summary>
    public string? BasePath { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets the security scheme to be defined for the output.
    /// </summary>
    public SwaggerDocumentSecurityDefinitions? SecurityDefinitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the security options available in the output.
    /// </summary>
    public List<SwaggerDocumentSecurityRequirement>? Security { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration for the document's description.
    /// </summary>
    public SwaggerOutputInfoConfiguration? Info { get; set; }
}