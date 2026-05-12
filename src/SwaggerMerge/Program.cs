namespace SwaggerMerge;

using CommandLine;
using SwaggerMerge.V2.Document;
using Features.Merger;
using Infrastructure.Configuration;
using Infrastructure.Configuration.Logging;
using Infrastructure.Configuration.Merge;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static IHost? Host { get; set; }

    public static async Task Main(string[] args)
    {
        SerilogConfigurator.ConfigureLogging();
        Host = CreateHostBuilder(args).Build();

        await Parser.Default.ParseArguments<ConsoleOptions>(args)
            .WithParsedAsync(async options => await GetRequiredService<ISwaggerMerger>()
                .RunAsync(options.ConfigPath ?? throw new InvalidOperationException("ConfigPath is null")));
    }

    private static T GetRequiredService<T>()
        where T : notnull
    {
        if (Host is null)
        {
            throw new InvalidOperationException("Host is not initialized");
        }

        return Host.Services.GetRequiredService<T>();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) => builder.SetBasePath(Directory.GetCurrentDirectory()))
            .ConfigureServices((_, services) =>
            {
                services.AddLogging();
                services.AddTransient<SwaggerMerge.V2.Document.ISwaggerDocumentHandler, SwaggerMerge.V2.Document.SwaggerDocumentHandler>();
                services.AddTransient<SwaggerMerge.V3.Document.IOpenApiDocumentHandler, SwaggerMerge.V3.Document.OpenApiDocumentHandler>();
                services.AddTransient<ISwaggerMergeConfigurationFileHandler, SwaggerMergeConfigurationFileHandler>();
                services.AddTransient<SwaggerMerge.V2.ISwaggerMergeHandler, SwaggerMerge.V2.SwaggerMergeHandler>();
                services.AddTransient<SwaggerMerge.V3.IOpenApiMergeHandler, SwaggerMerge.V3.OpenApiMergeHandler>();
                services.AddTransient<ISwaggerMerger, SwaggerMerger>();
            });

        return hostBuilder;
    }
}
