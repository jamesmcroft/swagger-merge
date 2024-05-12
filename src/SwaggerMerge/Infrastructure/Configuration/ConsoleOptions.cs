namespace SwaggerMerge.Infrastructure.Configuration;

using CommandLine;

internal sealed class ConsoleOptions
{
    [Option('c', "config", Required = true, HelpText = "The path to the Swagger Merge configuration file.")]
    public string? ConfigPath { get; set; }
}
