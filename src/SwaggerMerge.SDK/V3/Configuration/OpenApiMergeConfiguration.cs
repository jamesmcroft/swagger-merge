namespace SwaggerMerge.V3.Configuration;

using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Configuration.Output;

/// <summary>
/// Defines the configuration for merging OpenAPI V3 documents.
/// </summary>
public class OpenApiMergeConfiguration
{
    /// <summary>
    /// Gets or sets the inputs for merging.
    /// </summary>
    public IEnumerable<OpenApiInputConfiguration> Inputs { get; set; } = new List<OpenApiInputConfiguration>();

    /// <summary>
    /// Gets or sets the output merged OpenAPI document.
    /// </summary>
    public OpenApiOutputConfiguration Output { get; set; } = new();
}
