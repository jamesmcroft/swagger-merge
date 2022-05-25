namespace SwaggerMerge.Merge;

using MADE.Collections;
using Swagger;
using SwaggerMerge.Merge.Configuration.Input;

internal static partial class SwaggerMerger
{
    private static void UpdateOutputPathsFromInput(
        SwaggerDocument output,
        SwaggerInputConfiguration inputConfig,
        SwaggerDocument input)
    {
        if (input.Paths == null)
        {
            return;
        }

        var pathsToProcess = DetermineOutputPathsFromInput(input, inputConfig);
        if (pathsToProcess == null)
        {
            return;
        }

        foreach (var path in pathsToProcess)
        {
            var outputPath = path.Key;

            outputPath = StripStartFromPath(outputPath, inputConfig);
            outputPath = PrependToPath(outputPath, inputConfig);
            output.Paths.AddOrUpdate(outputPath, path.Value);
        }
    }

    private static string UpdateOutputTitleFromInput(
        string outputTitle,
        SwaggerInputConfiguration inputConfig,
        SwaggerDocument input)
    {
        if (inputConfig.Info is not { Append: true })
        {
            // Input title is not to be appended to the output title.
            return outputTitle;
        }

        if (inputConfig.Info.Title != null && !string.IsNullOrWhiteSpace(inputConfig.Info.Title))
        {
            outputTitle += " " + inputConfig.Info.Title;
        }
        else
        {
            outputTitle += input.Info.Title;
        }

        return outputTitle;
    }

    private static void UpdateOutputDefinitionsFromInput(
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

    private static SwaggerDocumentPaths? DetermineOutputPathsFromInput(SwaggerDocument input, SwaggerInputConfiguration inputConfig)
    {
        if (input.Paths == null)
        {
            return null;
        }

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

    private static string StripStartFromPath(string path, SwaggerInputConfiguration inputConfig)
    {
        if (inputConfig.Path?.StripStart != null && !string.IsNullOrWhiteSpace(inputConfig.Path.StripStart))
        {
            path = path.Substring(inputConfig.Path.StripStart.Length);
        }

        return path;
    }

    private static string PrependToPath(string path, SwaggerInputConfiguration inputConfig)
    {
        if (inputConfig.Path?.Prepend != null && !string.IsNullOrWhiteSpace(inputConfig.Path.Prepend))
        {
            path = inputConfig.Path.Prepend + path;
        }

        return path;
    }
}