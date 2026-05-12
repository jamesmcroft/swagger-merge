namespace SwaggerMerge.V3;

using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Document;

/// <summary>
/// Defines an implementation for merging OpenAPI V3 documents.
/// </summary>
public partial class OpenApiMergeHandler : IOpenApiMergeHandler
{
    /// <summary>
    /// Merges OpenAPI documents together based on the given merge configuration.
    /// </summary>
    /// <param name="config">The configuration that contains the detail of the inputs and outputs.</param>
    /// <returns>The merged OpenAPI document.</returns>
    public OpenApiDocument Merge(OpenApiMergeConfiguration config)
    {
        var output = new OpenApiDocument();

        var outputTitle = config.Output.Info?.Title ?? string.Empty;
        var highestVersion = "3.0.0";

        foreach (var inputConfig in config.Inputs)
        {
            var input = inputConfig.File;

            if (input == null)
            {
                continue;
            }

            highestVersion = GetHighestVersion(highestVersion, input.OpenApiVersion);
            outputTitle = UpdateOutputTitleFromInput(outputTitle, inputConfig, input);
            UpdateOutputPathsFromInput(output, inputConfig, input);
            UpdateOutputComponentSchemasFromInput(output, input);
        }

        FinalizeOutput(output, outputTitle, highestVersion, config);

        return output;
    }

    private static string GetHighestVersion(string current, string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return current;
        }

        if (Version.TryParse(candidate, out var candidateVersion) &&
            Version.TryParse(current, out var currentVersion))
        {
            return candidateVersion > currentVersion ? candidate : current;
        }

        return current;
    }

    private static void FinalizeOutput(
        OpenApiDocument? output,
        string outputTitle,
        string openApiVersion,
        OpenApiMergeConfiguration config)
    {
        if (output == null)
        {
            return;
        }

        // Where exclusions have been specified, remove any component schemas from the output where they are no longer valid
        if (config.Inputs.Any(x => x.Path is { OperationExclusions: not null } && x.Path.OperationExclusions.Any())
            && output.Components?.Schemas != null)
        {
            output.Components.Schemas = GetUsedComponentSchemas(output);
        }

        output.OpenApiVersion = openApiVersion;
        output.Info.Title = outputTitle;
        output.Info.Version = config.Output.Info?.Version ?? "1.0";
        output.Servers = config.Output.Servers;
        output.Components ??= new OpenApiComponents();
        output.Components.SecuritySchemes = config.Output.SecuritySchemes ?? new OpenApiDocumentSecurityDefinitions();
        output.Security = config.Output.Security ?? new List<OpenApiDocumentSecurityRequirement>();
    }
}
