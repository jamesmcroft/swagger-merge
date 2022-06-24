namespace SwaggerMerge.Features.Merger;

internal interface ISwaggerMerger
{
    Task RunAsync(string configFilePath);
}