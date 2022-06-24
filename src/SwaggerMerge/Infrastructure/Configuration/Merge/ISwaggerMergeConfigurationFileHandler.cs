namespace SwaggerMerge.Infrastructure.Configuration.Merge;

using SwaggerMerge.Configuration;

internal interface ISwaggerMergeConfigurationFileHandler
{
    Task<SwaggerMergeConfigurationFile> LoadAsync(string configFilePath);

    SwaggerMergeConfigurationFileValidationResult Validate(SwaggerMergeConfigurationFile config);

    Task<SwaggerMergeConfiguration> ConvertAsync(SwaggerMergeConfigurationFile config);
}