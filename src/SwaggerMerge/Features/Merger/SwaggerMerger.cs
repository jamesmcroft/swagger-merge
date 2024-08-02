namespace SwaggerMerge.Features.Merger;

using Document;
using Infrastructure.Configuration.Merge;
using Serilog;
using SwaggerMerge;

internal sealed class SwaggerMerger(
    ISwaggerMergeConfigurationFileHandler configurationFileHandler,
    ISwaggerMergeHandler mergeHandler,
    ISwaggerDocumentHandler documentHandler)
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

            var config = await configurationFileHandler.ConvertAsync(configFile);

            Log.Information("Merging {InputsCount} Swagger documents...", config.Inputs.Count());
            var output = mergeHandler.Merge(config);

            Log.Information("Saving output Swagger document to '{OutputFilePath}'...", configFile.Output.File);
            await documentHandler.SaveToPathAsync(output, configFile.Output.File);

            Log.Information(
                "Finished merging {InputsCount} Swagger documents to '{OutputFilePath}'!",
                configFile.Inputs.Count(),
                configFile.Output.File);
        }
        catch (Exception e)
        {
            Log.Error(e, "An exception was thrown while merging Swagger documents");
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
