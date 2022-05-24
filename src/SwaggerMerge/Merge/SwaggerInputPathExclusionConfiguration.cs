namespace SwaggerMerge.Merge;

/// <summary>
/// Defines the configuration for modifying a Swagger document's path exclusions.
/// </summary>
public class SwaggerInputPathExclusionConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to exclude path method implementations marked as internal.
    /// </summary>
    public bool Internal { get; set; } = false;
}