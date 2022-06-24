namespace SwaggerMerge;

using Configuration;
using Document;

/// <summary>
/// Defines an interface for performing the merge of Swagger documents.
/// </summary>
public interface ISwaggerMergeHandler
{
    /// <summary>
    /// Merges Swagger documents together based on the given merge configuration.
    /// </summary>
    /// <param name="config">The configuration that contains the detail of the inputs and outputs.</param>
    /// <returns>The merged Swagger document.</returns>
    SwaggerDocument Merge(SwaggerMergeConfiguration config);
}