namespace SwaggerMerge.Merge;

using SwaggerMerge.Merge.Configuration;
using SwaggerMerge.Serialization;
using SwaggerMerge.Swagger;

internal static partial class SwaggerMerger
{
    public static async Task MergeAsync(SwaggerMergeConfiguration config)
    {
        var output = new SwaggerDocument { Host = config.Output.Host, BasePath = config.Output.BasePath };

        var outputTitle = config.Output.Info?.Title ?? string.Empty;

        foreach (var inputConfig in config.Inputs)
        {
            var input = await JsonFile.LoadFileAsync<SwaggerDocument>(inputConfig.File);

            outputTitle = UpdateOutputTitleFromInput(outputTitle, inputConfig, input);
            UpdateOutputPathsFromInput(output, inputConfig, input);
            UpdateOutputDefinitionsFromInput(output, input);
        }

        FinalizeOutput(output, outputTitle, config);

        await JsonFile.SaveFileAsync(config.Output.File, output);

        Console.WriteLine($"Merged {config.Inputs.Count()} files into '{config.Output.File}'");
    }

    private static void FinalizeOutput(SwaggerDocument? output, string outputTitle, SwaggerMergeConfiguration config)
    {
        if (output == null)
        {
            return;
        }

        // Where exclusions have been specified, remove any definitions from the output where they are no longer valid
        if (config.Inputs.Any(x => x.Path is { OperationExclusions: { } } && x.Path.OperationExclusions.Any()))
        {
            if (output.Definitions != null)
            {
                output.Definitions = GetUsedDefinitions(output);
            }
        }

        output.Info.Title = outputTitle;
        output.Info.Version = config.Output.Info?.Version ?? "1.0";
        output.Schemes = config.Output.Schemes ?? new List<string>();
        output.SecurityDefinitions = config.Output.SecurityDefinitions ?? new SwaggerDocumentSecurityDefinitions();
        output.Security = config.Output.Security ?? new List<SwaggerDocumentSecurityRequirement>();
    }
}