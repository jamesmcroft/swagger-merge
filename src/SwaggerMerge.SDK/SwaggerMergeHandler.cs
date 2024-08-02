namespace SwaggerMerge;

using Configuration;
using Document;

/// <summary>
/// Defines an implementation for merging Swagger documents using the V2 format.
/// </summary>
public partial class SwaggerMergeHandler : ISwaggerMergeHandler
{
    /// <summary>
    /// Merges Swagger documents together based on the given merge configuration.
    /// </summary>
    /// <param name="config">The configuration that contains the detail of the inputs and outputs.</param>
    /// <returns>The merged Swagger document.</returns>
    public SwaggerDocument Merge(SwaggerMergeConfiguration config)
    {
        var output = new SwaggerDocument { Host = config.Output.Host, BasePath = config.Output.BasePath };

        var outputTitle = config.Output.Info?.Title ?? string.Empty;

        foreach (var inputConfig in config.Inputs)
        {
            var input = inputConfig.File;

            if (input == null)
            {
                continue;
            }

            outputTitle = UpdateOutputTitleFromInput(outputTitle, inputConfig, input);
            UpdateOutputPathsFromInput(output, inputConfig, input);
            UpdateOutputDefinitionsFromInput(output, input);
        }

        FinalizeOutput(output, outputTitle, config);

        return output;
    }

    private static void FinalizeOutput(SwaggerDocument? output, string outputTitle, SwaggerMergeConfiguration config)
    {
        if (output == null)
        {
            return;
        }

        // Where exclusions have been specified, remove any definitions from the output where they are no longer valid
        if (config.Inputs.Any(x => x.Path is { OperationExclusions: not null } && x.Path.OperationExclusions.Any())
            && output.Definitions != null)
        {
            output.Definitions = GetUsedDefinitions(output);
        }

        output.Info.Title = outputTitle;
        output.Info.Version = config.Output.Info?.Version ?? "1.0";
        output.Schemes = config.Output.Schemes ?? new List<string>();
        output.SecurityDefinitions = config.Output.SecurityDefinitions ?? new SwaggerDocumentSecurityDefinitions();
        output.Security = config.Output.Security ?? new List<SwaggerDocumentSecurityRequirement>();
    }
}
