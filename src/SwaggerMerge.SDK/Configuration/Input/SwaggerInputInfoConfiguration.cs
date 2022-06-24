namespace SwaggerMerge.Configuration.Input;

/// <summary>
/// Defines the configuration for modifying a Swagger document's description.
/// </summary>
public class SwaggerInputInfoConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to append the document title.
    /// </summary>
    public bool Append { get; set; } = false;

    /// <summary>
    /// Gets or sets the title to append if <see cref="Append"/> is <b>true</b>.
    /// </summary>
    /// <remarks>
    /// If not set, the Swagger document's title will be used.
    /// </remarks>
    public string? Title { get; set; }
}