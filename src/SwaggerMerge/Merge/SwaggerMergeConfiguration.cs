namespace SwaggerMerge.Merge;

/// <summary>
/// Defines the configuration for merging Swagger documents.
/// </summary>
public class SwaggerMergeConfiguration
{
    /// <summary>
    /// Gets or sets the inputs for merging.
    /// </summary>
    public IEnumerable<SwaggerInputConfiguration> Inputs { get; set; }

    /// <summary>
    /// Gets or sets the output merged Swagger document.
    /// </summary>
    public SwaggerOutputConfiguration Output { get; set; }
}