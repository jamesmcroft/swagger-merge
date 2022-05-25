namespace SwaggerMerge.Merge;

using Exceptions;
using MADE.Collections;
using SwaggerMerge.Serialization;
using SwaggerMerge.Swagger;

internal static class SwaggerMerger
{
    public static async Task MergeAsync(SwaggerMergeConfiguration config)
    {
        var output = new SwaggerDocument { Host = config.Output.Host, BasePath = config.Output.BasePath };

        var outputTitle = config.Output.Info?.Title ?? string.Empty;

        foreach (var inputConfig in config.Inputs)
        {
            var input = await JsonFile.LoadFileAsync<SwaggerDocument>(inputConfig.File);

            outputTitle = ProcessOutputTitle(outputTitle, inputConfig, input);
            ProcessInputPaths(output, inputConfig, input);
            ProcessInputDefinitions(output, input);
        }

        ProcessOutputDefinitions(config.Output, output);

        output.Info.Title = outputTitle;
        output.Info.Version = config.Output.Info?.Version ?? "1.0";
        output.Schemes = config.Output.Schemes ?? new List<string>();
        output.SecurityDefinitions = config.Output.SecurityDefinitions ?? new SwaggerDocumentSecurityDefinitions();
        output.Security = config.Output.Security ?? new List<SwaggerDocumentSecurityRequirement>();

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

    private static void ProcessInputPaths(
        SwaggerDocument output,
        SwaggerInputConfiguration inputConfig,
        SwaggerDocument input)
    {
        SwaggerDocumentPaths DetermineOutputPaths()
        {
            var swaggerDocumentPaths = input.Paths;

            foreach (var (path, pathOperations) in input.Paths)
            {
                if (inputConfig.Path is not { OperationExclusions: { } } || !inputConfig.Path.OperationExclusions.Any())
                {
                    continue;
                }

                foreach (var (method, operation) in pathOperations)
                {
                    // Remove any paths where the additional properties are included in the exclusions.
                    foreach (var (_, _) in inputConfig.Path.OperationExclusions.Where(
                                 pathOperationExclusion => operation.AdditionalProperties != null &&
                                                           operation.AdditionalProperties.ContainsKey(
                                                               pathOperationExclusion.Key) &&
                                                           operation.AdditionalProperties[
                                                                   pathOperationExclusion.Key]
                                                               .Equals(pathOperationExclusion.Value)))
                    {
                        pathOperations.Remove(method);
                    }
                }

                if (!pathOperations.Any())
                {
                    swaggerDocumentPaths.Remove(path);
                }
                else
                {
                    swaggerDocumentPaths[path] = pathOperations;
                }
            }

            return swaggerDocumentPaths;
        }

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

        if (input.Paths == null)
        {
            return;
        }

        var pathsToProcess = DetermineOutputPaths();

        foreach (var path in pathsToProcess)
        {
            var outputPath = path.Key;

            outputPath = StripOutputPathStart(outputPath);
            outputPath = PrependOutputPath(outputPath);
            output.Paths.AddOrUpdate(outputPath, path.Value);
        }
    }

    private static void ProcessInputDefinitions(
        SwaggerDocument output,
        SwaggerDocument input)
    {
        if (input.Definitions == null || output.Definitions == null)
        {
            return;
        }

        foreach (var definition in input.Definitions.Where(definition =>
                     !output.Definitions.ContainsKey(definition.Key)))
        {
            output.Definitions.AddOrUpdate(definition.Key, definition.Value);
        }
    }

    private static void ProcessOutputDefinitions(
        SwaggerOutputConfiguration outputConfig,
        SwaggerDocument output)
    {
        if (outputConfig.InlineSchema && output.Definitions != null)
        {
            foreach (var definition in output.Definitions)
            {
                if (definition.Value.Properties != null)
                {
                    foreach (var definitionProp in definition.Value.Properties)
                    {
                        if (definitionProp.Value.Reference != null)
                        {
                            var expectedReference = output.Definitions.FirstOrDefault(x =>
                                x.Key.Equals(definitionProp.Value.Reference.Replace("#/definitions/", string.Empty), StringComparison.CurrentCultureIgnoreCase));


                        }
                    }
                }
            }
        }
    }
}