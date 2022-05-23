namespace SwaggerMerge.Merge;

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
    /// Gets or sets the configuration for the document's description.
    /// </summary>
    public SwaggerOutputInfoConfiguration? Info { get; set; }
}