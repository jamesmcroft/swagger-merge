namespace SwaggerMerge.Merge.Configuration.Input;

/// <summary>
/// Defines the configuration for a Swagger document input.
/// </summary>
public class SwaggerInputConfiguration
{
    /// <summary>
    /// Gets or sets the file path to the Swagger document.
    /// </summary>
    public string File { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the configuration for modifying the document's paths.
    /// </summary>
    public SwaggerInputPathConfiguration? Path { get; set; }

    /// <summary>
    /// Gets or sets the configuration for modifying the document's description.
    /// </summary>
    public SwaggerInputInfoConfiguration? Info { get; set; }
}