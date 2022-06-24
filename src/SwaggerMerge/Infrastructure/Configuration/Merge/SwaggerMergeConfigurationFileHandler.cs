namespace SwaggerMerge.Infrastructure.Configuration.Merge;

using System.Text;
using Document;
using Exceptions;
using Newtonsoft.Json;
using SwaggerMerge.Configuration;
using SwaggerMerge.Configuration.Input;

internal class SwaggerMergeConfigurationFileHandler : ISwaggerMergeConfigurationFileHandler
{
    private readonly ISwaggerDocumentHandler documentHandler;

    public SwaggerMergeConfigurationFileHandler(ISwaggerDocumentHandler documentHandler)
    {
        this.documentHandler = documentHandler;
    }

    public async Task<SwaggerMergeConfigurationFile> LoadAsync(string configFilePath)
    {
        var content = await ReadAllTextAsync(configFilePath);
        var config = JsonConvert.DeserializeObject<SwaggerMergeConfigurationFile>(content);

        if (config == null)
        {
            throw new SwaggerMergeException(
                $"The Swagger merge configuration file at '{configFilePath}' could not be loaded correctly as it is not in the correct format.");
        }

        Directory.SetCurrentDirectory(Path.GetDirectoryName(configFilePath));

        return config;
    }

    public SwaggerMergeConfigurationFileValidationResult Validate(SwaggerMergeConfigurationFile config)
    {
        var isValid = true;
        var errors = new List<string>();

        if (!config.Inputs.Any())
        {
            isValid = false;
            errors.Add("At least one input must be defined");
        }

        if (config.Inputs.Any(input => string.IsNullOrWhiteSpace(input.File)))
        {
            isValid = false;
            errors.Add("All inputs must have a file path defined");
        }

        if (string.IsNullOrWhiteSpace(config.Output.File))
        {
            isValid = false;
            errors.Add("The output file must be defined");
        }

        return new SwaggerMergeConfigurationFileValidationResult(isValid, errors);
    }

    public async Task<SwaggerMergeConfiguration> ConvertAsync(SwaggerMergeConfigurationFile config)
    {
        var inputs = new List<SwaggerInputConfiguration>();

        foreach (var input in config.Inputs)
        {
            var inputFile = await this.documentHandler.LoadFromFilePathAsync(input.File);
            inputs.Add(new SwaggerInputConfiguration { File = inputFile, Path = input.Path, Info = input.Info });
        }

        return new SwaggerMergeConfiguration { Inputs = inputs, Output = config.Output };
    }

    private static async Task<string> ReadAllTextAsync(string filePath)
    {
        using var stream = new StreamReader(filePath, Encoding.UTF8);
        return await stream.ReadToEndAsync();
    }
}