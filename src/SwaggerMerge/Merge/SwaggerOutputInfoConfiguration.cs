namespace SwaggerMerge.Merge;

/// <summary>
/// Defines the configuration for modifying the output Swagger document's description.
/// </summary>
public class SwaggerOutputInfoConfiguration
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