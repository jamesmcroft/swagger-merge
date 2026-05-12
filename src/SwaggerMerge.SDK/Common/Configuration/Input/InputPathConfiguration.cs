namespace SwaggerMerge.Common.Configuration.Input;

/// <summary>
/// Defines the configuration for modifying a Swagger document's paths.
/// </summary>
public class InputPathConfiguration
{
    /// <summary>
    /// Gets or sets the value to string from the start of paths.
    /// </summary>
    public string? StripStart { get; set; }

    /// <summary>
    /// Gets or sets the value to prepend to the start of paths.
    /// </summary>
    public string? Prepend { get; set; }

    /// <summary>
    /// Gets or sets the exclusions for the output for path operation implementations based on a key-value match.
    /// </summary>
    public InputPathOperationExclusionConfiguration? OperationExclusions { get; set; }
}
