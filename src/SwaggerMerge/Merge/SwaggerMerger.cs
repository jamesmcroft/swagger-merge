namespace SwaggerMerge.Merge;

using Exceptions;
using MADE.Collections;
using SwaggerMerge.Serialization;
using SwaggerMerge.Swagger;

internal static class SwaggerMerger
{
    public static async Task MergeAsync(SwaggerMergeConfiguration config)
    {
        var output = new SwaggerDocument
        {
            Host = config.Output.Host,
            BasePath = config.Output.BasePath
        };

        var outputTitle = config.Output.Info?.Title ?? string.Empty;

        foreach (var inputConfig in config.Inputs)
        {
            var input = await JsonFile.LoadFileAsync<SwaggerDocument>(inputConfig.File);

            outputTitle = ProcessOutputTitle(outputTitle, inputConfig, input);
            ProcessOutputPaths(output, inputConfig, input);
            ProcessOutputDefinitions(output, input);
        }

        output.Info.Title = outputTitle;
        output.Info.Version = config.Output.Info?.Version ?? "1.0";

        await JsonFile.SaveFileAsync(config.Output.File, output);

        Console.WriteLine($"Merged {config.Inputs.Count()} files into '{config.Output.File}'");
    }

    public static void ValidateConfiguration(SwaggerMergeConfiguration config)
    {
        if (!config.Inputs.Any())
        {
            throw new SwaggerMergeException("At least 1 input file must be specified");
        }

        if (config.Inputs.Any(input => string.IsNullOrWhiteSpace(input.File)))
        {
            throw new SwaggerMergeException("All input file paths must be specified");
        }

        if (string.IsNullOrWhiteSpace(config.Output.File))
        {
            throw new SwaggerMergeException("The output file path must be specified");
        }
    }

    private static string ProcessOutputTitle(
        string outputTitle,
        SwaggerInputConfiguration inputConfig,
        SwaggerDocument input)
    {
        if (inputConfig.Info is not { Append: true })
        {
            return outputTitle;
        }

        if (inputConfig.Info.Title != null &&
            !string.IsNullOrWhiteSpace(inputConfig.Info.Title))
        {
            outputTitle += " " + inputConfig.Info.Title;
        }
        else
        {
            outputTitle += input.Info.Title;
        }

        return outputTitle;
    }

    private static void ProcessOutputPaths(
        SwaggerDocument output,
        SwaggerInputConfiguration inputConfig,
        SwaggerDocument input)
    {
        string PrependOutputPath(string outputPath)
        {
            if (inputConfig.Path?.Prepend != null && !string.IsNullOrWhiteSpace(inputConfig.Path.Prepend))
            {
                outputPath = inputConfig.Path.Prepend + outputPath;
            }

            return outputPath;
        }

        string StripOutputPathStart(string outputPath)
        {
            if (inputConfig.Path?.StripStart != null && !string.IsNullOrWhiteSpace(inputConfig.Path.StripStart))
            {
                outputPath = outputPath.Substring(inputConfig.Path.StripStart.Length);
            }

            return outputPath;
        }

        foreach (var path in input.Paths)
        {
            var outputPath = path.Key;

            outputPath = StripOutputPathStart(outputPath);
            outputPath = PrependOutputPath(outputPath);
            output.Paths.AddOrUpdate(outputPath, path.Value);
        }
    }

    private static void ProcessOutputDefinitions(SwaggerDocument output, SwaggerDocument input)
    {
        if (input.Definitions != null && output.Definitions != null)
        {
            foreach (var definition in input.Definitions.Where(definition =>
                         !output.Definitions.ContainsKey(definition.Key)))
            {
                output.Definitions.AddOrUpdate(definition.Key, definition.Value);
            }
        }
    }
}