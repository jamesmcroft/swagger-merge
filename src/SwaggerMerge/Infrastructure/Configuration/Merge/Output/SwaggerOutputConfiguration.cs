namespace SwaggerMerge.Infrastructure.Configuration.Merge.Output;

using SwaggerMerge.V3.Document;

/// <summary>
/// Defines the configuration for a Swagger/OpenAPI document output.
/// Extends the V2 SDK output configuration with the output file path and V3-specific fields.
/// </summary>
public class SwaggerOutputConfiguration : SwaggerMerge.V2.Configuration.Output.SwaggerOutputConfiguration
{
    /// <summary>
    /// Gets or sets the file path of the output merged document.
    /// </summary>
    public string File { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the server objects providing connectivity information (OpenAPI V3 only).
    /// </summary>
    public List<OpenApiServer>? Servers { get; set; }
}