namespace SwaggerMerge.Features.Merger;

using Document;
using Infrastructure.Configuration.Merge;
using Serilog;
using SwaggerMerge;

internal class SwaggerMerger : ISwaggerMerger
{
    private readonly ISwaggerMergeConfigurationFileHandler configurationFileHandler;
    private readonly ISwaggerMergeHandler mergeHandler;
    private readonly ISwaggerDocumentHandler documentHandler;

    public SwaggerMerger(
        ISwaggerMergeConfigurationFileHandler configurationFileHandler,
        ISwaggerMergeHandler mergeHandler,
        ISwaggerDocumentHandler documentHandler)
    {
        this.configurationFileHandler = configurationFileHandler;
        this.mergeHandler = mergeHandler;
        this.documentHandler = documentHandler;
    }

    public async Task RunAsync(string configFilePath)
    {
        try
        {
            Log.Information("Loading configuration file from '{ConfigFilePath}'...", configFilePath);
            var configFile = await this.configurationFileHandler.LoadAsync(configFilePath);
            var result = this.configurationFileHandler.Validate(configFile);

            if (!result.IsValid)
            {
                LogValidationErrors(result);
                return;
            }

            var config = await this.configurationFileHandler.ConvertAsync(configFile);

            Log.Information("Merging {InputsCount} Swagger documents...", config.Inputs.Count());
            var output = this.mergeHandler.Merge(config);

            Log.Information("Saving output Swagger document to '{OutputFilePath}'...", configFile.Output.File);
            await this.documentHandler.SaveToPathAsync(output, configFile.Output.File);

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