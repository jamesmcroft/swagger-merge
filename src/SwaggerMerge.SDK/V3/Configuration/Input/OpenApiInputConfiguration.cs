namespace SwaggerMerge.V3.Configuration.Input;

using SwaggerMerge.Common.Configuration.Input;

using SwaggerMerge.V3.Document;

/// <summary>
/// Defines the configuration for an OpenAPI V3 document input.
/// </summary>
public class OpenApiInputConfiguration
{
    /// <summary>
    /// Gets or sets the OpenAPI document.
    /// </summary>
    public OpenApiDocument? File { get; set; }

    /// <summary>
    /// Gets or sets the configuration for modifying the document's paths.
    /// </summary>
    public InputPathConfiguration? Path { get; set; }

    /// <summary>
    /// Gets or sets the configuration for modifying the document's description.
    /// </summary>
    public InputInfoConfiguration? Info { get; set; }
}
