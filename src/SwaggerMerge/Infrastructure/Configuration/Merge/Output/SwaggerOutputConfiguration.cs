namespace SwaggerMerge.Infrastructure.Configuration.Merge.Output;

/// <summary>
/// Defines the configuration for a Swagger document output.
/// </summary>
public class SwaggerOutputConfiguration : SwaggerMerge.Configuration.Output.SwaggerOutputConfiguration
{
    /// <summary>
    /// Gets or sets the file path of the output merged Swagger document.
    /// </summary>
    public string File { get; set; } = string.Empty;
}