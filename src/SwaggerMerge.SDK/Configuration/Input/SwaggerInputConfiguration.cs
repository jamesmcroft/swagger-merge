namespace SwaggerMerge.Configuration.Input;

using Document;

/// <summary>
/// Defines the configuration for a Swagger document input.
/// </summary>
public class SwaggerInputConfiguration
{
    /// <summary>
    /// Gets or sets the Swagger document.
    /// </summary>
    public SwaggerDocument? File { get; set; }

    /// <summary>
    /// Gets or sets the configuration for modifying the document's paths.
    /// </summary>
    public SwaggerInputPathConfiguration? Path { get; set; }

    /// <summary>
    /// Gets or sets the configuration for modifying the document's description.
    /// </summary>
    public SwaggerInputInfoConfiguration? Info { get; set; }
}
