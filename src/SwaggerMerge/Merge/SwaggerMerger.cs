namespace SwaggerMerge.Merge;

using Exceptions;
using MADE.Collections;
using SwaggerMerge.Serialization;
using SwaggerMerge.Swagger;

internal static class SwaggerMerger
{
    public static async Task MergeAsync(SwaggerMergeConfiguration config)
    {
        var output = new SwaggerDocument {Host = config.Output.Host, BasePath = config.Output.BasePath};

        var outputTitle = config.Output.Info?.Title ?? string.Empty;

        foreach (var inputConfig in config.Inputs)
        {
            var input = await JsonFile.LoadFileAsync<SwaggerDocument>(inputConfig.File);

            outputTitle = ProcessOutputTitle(outputTitle, inputConfig, input);
            ProcessInputPaths(output, inputConfig, input);
            ProcessInputDefinitions(output, input);
        }

        if (config.Inputs.Any(x => x.Path is {OperationExclusions: { }} && x.Path.OperationExclusions.Any()))
        {
            if (output.Definitions != null)
            {
                output.Definitions = RemoveUnusedDefinitions(output);
            }
        }

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
        if (inputConfig.Info is not {Append: true})
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
                if (inputConfig.Path is not {OperationExclusions: { }} || !inputConfig.Path.OperationExclusions.Any())
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

    private static SwaggerDocumentDefinitions RemoveUnusedDefinitions(SwaggerDocument document)
    {
        void ProcessDefinitionPropertyItemReferences(KeyValuePair<string, SwaggerDocumentSchema> property,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (property.Value.Items?.Reference == null)
            {
                return;
            }

            var expectedDefinition = GetDefinitionByReference(document.Definitions, property.Value.Items.Reference);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || allDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            allDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessReferenceDefinitionReferences(expectedDefinition, allDefinitions);
        }

        void ProcessDefinitionPropertyReferences(KeyValuePair<string, SwaggerDocumentSchema> property,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (property.Value.Reference == null)
            {
                return;
            }

            var expectedDefinition = GetDefinitionByReference(document.Definitions, property.Value.Reference);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || allDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            allDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessReferenceDefinitionReferences(expectedDefinition, allDefinitions);
        }

        void ProcessReferenceDefinitionReferences(
            KeyValuePair<string, SwaggerDocumentSchema> definition,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (definition.Value.Properties == null)
            {
                return;
            }

            foreach (var property in definition.Value.Properties)
            {
                ProcessDefinitionPropertyReferences(property, allDefinitions);

                ProcessDefinitionPropertyItemReferences(property, allDefinitions);
            }
        }

        void ProcessResponseSchemaItemReferences(
            SwaggerDocumentResponse response,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (response.Schema is not {Items.Reference: { }})
            {
                return;
            }

            var expectedDefinition =
                GetDefinitionByReference(document.Definitions, response.Schema?.Reference);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || allDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            allDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessReferenceDefinitionReferences(expectedDefinition, allDefinitions);
        }

        void ProcessResponseSchemaReferences(
            SwaggerDocumentResponse response,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (response.Schema is not {Reference: { }})
            {
                return;
            }

            var expectedDefinition =
                GetDefinitionByReference(document.Definitions, response.Schema?.Reference);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || allDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            allDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessReferenceDefinitionReferences(expectedDefinition, allDefinitions);
        }

        void ProcessParameterSchemaItemReferences(
            SwaggerDocumentOperation operation,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var referenceParameter in operation.Parameters.Where(x =>
                         x.Schema is {Items.Reference: { }}))
            {
                var expectedDefinition =
                    GetDefinitionByReference(document.Definitions, referenceParameter.Schema?.Items?.Reference);

                if (string.IsNullOrWhiteSpace(expectedDefinition.Key) ||
                    allDefinitions.ContainsKey(expectedDefinition.Key))
                {
                    continue;
                }

                allDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
                ProcessReferenceDefinitionReferences(expectedDefinition, allDefinitions);
            }
        }

        void ProcessParameterSchemaReferences(
            SwaggerDocumentOperation operation,
            SwaggerDocumentDefinitions allDefinitions)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (SwaggerDocumentParameter referenceParameter in operation.Parameters.Where(x =>
                         x.Schema is {Reference: { }}))
            {
                var expectedDefinition =
                    GetDefinitionByReference(document.Definitions, referenceParameter.Schema?.Reference);

                if (string.IsNullOrWhiteSpace(expectedDefinition.Key) ||
                    allDefinitions.ContainsKey(expectedDefinition.Key))
                {
                    continue;
                }

                allDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
                ProcessReferenceDefinitionReferences(expectedDefinition, allDefinitions);
            }
        }

        SwaggerDocumentDefinitions definitions = new SwaggerDocumentDefinitions();

        if (document.Definitions == null || document.Paths == null)
        {
            return definitions;
        }

        foreach (var operation in document.Paths.SelectMany(path => path.Value.Select(method => method.Value)))
        {
            if (operation.Parameters != null)
            {
                ProcessParameterSchemaReferences(operation, definitions);
                ProcessParameterSchemaItemReferences(operation, definitions);
            }

            if (operation.Responses == null)
            {
                continue;
            }

            foreach (var response in operation.Responses.Select(operationResponse => operationResponse.Value))
            {
                ProcessResponseSchemaReferences(response, definitions);
                ProcessResponseSchemaItemReferences(response, definitions);
            }
        }

        return definitions;
    }

    private static KeyValuePair<string, SwaggerDocumentSchema> GetDefinitionByReference(
        SwaggerDocumentDefinitions definitions,
        string? reference)
    {
        var expectedDefinition = definitions.FirstOrDefault(x =>
            x.Key.Equals(
                reference?.Replace("#/definitions/", string.Empty),
                StringComparison.CurrentCultureIgnoreCase));
        return expectedDefinition;
    }
}