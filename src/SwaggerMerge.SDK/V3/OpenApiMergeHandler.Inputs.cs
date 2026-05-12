namespace SwaggerMerge.V3;

using SwaggerMerge.Common.Extensions;
using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Document;

/// <summary>
/// Defines the handler logic for merging OpenAPI V3 document inputs.
/// </summary>
public partial class OpenApiMergeHandler
{
    private static void UpdateOutputPathsFromInput(
        OpenApiDocument output,
        OpenApiInputConfiguration inputConfig,
        OpenApiDocument input)
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

        output.Paths ??= new OpenApiDocumentPaths();

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
        OpenApiInputConfiguration inputConfig,
        OpenApiDocument input)
    {
        if (inputConfig.Info is not { Append: true })
        {
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

    private static void UpdateOutputComponentSchemasFromInput(
        OpenApiDocument? output,
        OpenApiDocument input)
    {
        if (input.Components?.Schemas == null || output == null)
        {
            return;
        }

        output.Components ??= new OpenApiComponents();
        output.Components.Schemas ??= new Dictionary<string, OpenApiDocumentProperty>();

        foreach (var schema in input.Components.Schemas.Where(schema =>
                     !output.Components.Schemas.ContainsKey(schema.Key)))
        {
            output.Components.Schemas.AddOrUpdate(schema.Key, schema.Value);
        }
    }

    private static OpenApiDocumentPaths? DetermineOutputPathsFromInput(
        OpenApiDocument input,
        OpenApiInputConfiguration inputConfig)
    {
        if (input.Paths == null)
        {
            return null;
        }

        var documentPaths = input.Paths;

        foreach (var inputPath in input.Paths.ToList())
        {
            var path = inputPath.Key;
            var pathOperations = inputPath.Value;

            if (inputConfig.Path is not { OperationExclusions: not null } ||
                !inputConfig.Path.OperationExclusions.Any())
            {
                continue;
            }

            var methodsToRemove = (from pathOperation in pathOperations.ToList()
                                   let method = pathOperation.Key
                                   let operation = pathOperation.Value
                                   from exclusion in inputConfig.Path.OperationExclusions.Where(
                                       pathOperationExclusion => operation.JTokenProperties != null &&
                                                                 operation.JTokenProperties.ContainsKey(pathOperationExclusion.Key) &&
                                                                 operation.JTokenProperties[pathOperationExclusion.Key]
                                                                     .GetRawText() == pathOperationExclusion.Value.GetRawText())
                                   select method).ToList();

            foreach (var method in methodsToRemove)
            {
                pathOperations.Remove(method);
            }

            if (!pathOperations.Any())
            {
                documentPaths.Remove(path);
            }
            else
            {
                documentPaths[path] = pathOperations;
            }
        }

        return documentPaths;
    }

    private static string StripStartFromPath(string path, OpenApiInputConfiguration inputConfig)
    {
        if (inputConfig.Path?.StripStart != null && !string.IsNullOrWhiteSpace(inputConfig.Path.StripStart)
            && path.StartsWith(inputConfig.Path.StripStart, StringComparison.Ordinal))
        {
            path = path[inputConfig.Path.StripStart.Length..];
        }

        return path;
    }

    private static string PrependToPath(string path, OpenApiInputConfiguration inputConfig)
    {
        if (inputConfig.Path?.Prepend != null && !string.IsNullOrWhiteSpace(inputConfig.Path.Prepend))
        {
            path = inputConfig.Path.Prepend + path;
        }

        return path;
    }
}
