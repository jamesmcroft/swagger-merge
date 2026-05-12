namespace SwaggerMerge.Features.Merger;

using SwaggerMerge.Common.Document;
using SwaggerMerge.V2.Document;
using SwaggerMerge.V3.Document;
using Infrastructure.Configuration.Merge;
using Serilog;
using SwaggerMerge.V2;
using SwaggerMerge.V3;

internal sealed class SwaggerMerger(
    ISwaggerMergeConfigurationFileHandler configurationFileHandler,
    ISwaggerMergeHandler v2MergeHandler,
    IOpenApiMergeHandler v3MergeHandler,
    ISwaggerDocumentHandler v2DocumentHandler,
    IOpenApiDocumentHandler v3DocumentHandler)
    : ISwaggerMerger
{
    public async Task RunAsync(string configFilePath)
    {
        try
        {
            Log.Information("Loading configuration file from '{ConfigFilePath}'...", configFilePath);
            var configFile = await configurationFileHandler.LoadAsync(configFilePath);
            var result = configurationFileHandler.Validate(configFile);

            if (!result.IsValid)
            {
                LogValidationErrors(result);
                return;
            }

            var mergeConfig = await configurationFileHandler.ConvertWithVersionDetectionAsync(configFile);

            if (mergeConfig.Version == SpecVersion.OpenApiV3)
            {
                Log.Information("Detected OpenAPI V3 inputs. Merging {InputsCount} documents...", mergeConfig.V3Config!.Inputs.Count());
                var output = v3MergeHandler.Merge(mergeConfig.V3Config);

                Log.Information("Saving output OpenAPI document to '{OutputFilePath}'...", configFile.Output.File);
                await v3DocumentHandler.SaveToPathAsync(output, configFile.Output.File);

                Log.Information(
                    "Finished merging {InputsCount} OpenAPI documents to '{OutputFilePath}'!",
                    configFile.Inputs.Count(),
                    configFile.Output.File);
            }
            else
            {
                Log.Information("Detected Swagger V2 inputs. Merging {InputsCount} documents...", mergeConfig.V2Config!.Inputs.Count());
                var output = v2MergeHandler.Merge(mergeConfig.V2Config);

                Log.Information("Saving output Swagger document to '{OutputFilePath}'...", configFile.Output.File);
                await v2DocumentHandler.SaveToPathAsync(output, configFile.Output.File);

                Log.Information(
                    "Finished merging {InputsCount} Swagger documents to '{OutputFilePath}'!",
                    configFile.Inputs.Count(),
                    configFile.Output.File);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "An exception was thrown while merging documents");
        }
    }

    private static void LogValidationErrors(SwaggerMergeConfigurationFileValidationResult result)
    {
        if (result.Errors != null)
        {
            foreach (var error in result.Errors)
            {
                Log.Error(error);
            }
        }
        else
        {
            Log.Error("Unknown error occurred");
        }
    }
}
