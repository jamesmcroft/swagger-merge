namespace SwaggerMerge.V3.Configuration.Output;

using SwaggerMerge.V3.Document;

/// <summary>
/// Defines the configuration of an OpenAPI V3 document output.
/// </summary>
public class OpenApiOutputConfiguration
{
    /// <summary>
    /// Gets or sets the server objects providing connectivity information.
    /// </summary>
    public List<OpenApiServer>? Servers { get; set; }

    /// <summary>
    /// Gets or sets the security scheme to be defined for the output.
    /// </summary>
    public OpenApiDocumentSecurityDefinitions? SecuritySchemes { get; set; } = new();

    /// <summary>
    /// Gets or sets the security options available in the output.
    /// </summary>
    public List<OpenApiDocumentSecurityRequirement>? Security { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration for the document's description.
    /// </summary>
    public OpenApiOutputInfoConfiguration? Info { get; set; }
}
