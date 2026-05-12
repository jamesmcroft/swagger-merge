namespace SwaggerMerge.V3.Configuration.Output;

/// <summary>
/// Defines the configuration for modifying the output OpenAPI document's info metadata.
/// </summary>
public class OpenApiOutputInfoConfiguration
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    public string? Version { get; set; }
}
