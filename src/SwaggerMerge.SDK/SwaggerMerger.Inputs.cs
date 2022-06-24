namespace SwaggerMerge;

using Configuration.Input;
using Document;
using MADE.Collections;

/// <summary>
/// Defines the handler logic for merging Swagger document inputs.
/// </summary>
public partial class SwaggerMergeHandler
{
    private static void UpdateOutputPathsFromInput(
        SwaggerDocument? output,
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
            output?.Paths.AddOrUpdate(outputPath, path.Value);
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
        SwaggerDocument? output,
        SwaggerDocument input)
    {
        if (input.Definitions == null || output?.Definitions == null)
        {
            return;
        }

        foreach (var definition in input.Definitions.Where(definition =>
                     !output.Definitions.ContainsKey(definition.Key)))
        {
            output.Definitions.AddOrUpdate(definition.Key, definition.Value);
        }
    }

    private static SwaggerDocumentPaths? DetermineOutputPathsFromInput(
        SwaggerDocument input,
        SwaggerInputConfiguration inputConfig)
    {
        if (input.Paths == null)
        {
            return null;
        }

        var swaggerDocumentPaths = input.Paths;

        foreach (var inputPath in input.Paths)
        {
            var path = inputPath.Key;
            var pathOperations = inputPath.Value;

            if (inputConfig.Path is not { OperationExclusions: { } } || !inputConfig.Path.OperationExclusions.Any())
            {
                continue;
            }

            foreach (var method in from pathOperation in pathOperations
                     let method = pathOperation.Key
                     let operation = pathOperation.Value
                     from exclusion in inputConfig.Path.OperationExclusions.Where(
                         pathOperationExclusion => operation.JTokenProperties != null &&
                                                   operation.JTokenProperties.ContainsKey(pathOperationExclusion.Key) &&
                                                   operation.JTokenProperties[pathOperationExclusion.Key]
                                                       .Equals(pathOperationExclusion.Value))
                     select method)
            {
                pathOperations.Remove(method);
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