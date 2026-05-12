namespace SwaggerMerge.V3;

using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Document;

/// <summary>
/// Defines an interface for performing the merge of OpenAPI V3 documents.
/// </summary>
public interface IOpenApiMergeHandler
{
    /// <summary>
    /// Merges OpenAPI documents together based on the given merge configuration.
    /// </summary>
    /// <param name="config">The configuration that contains the detail of the inputs and outputs.</param>
    /// <returns>The merged OpenAPI document.</returns>
    OpenApiDocument Merge(OpenApiMergeConfiguration config);
}
