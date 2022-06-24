namespace SwaggerMerge;

using CommandLine;
using Document;
using Infrastructure.Configuration;
using Infrastructure.Configuration.Logging;
using Infrastructure.Configuration.Merge;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwaggerMerge.Features.Merger;

public class Program
{
    public static IHost Host { get; set; }

    public static async Task Main(string[] args)
    {
        SerilogConfigurator.ConfigureLogging();
        Host = CreateHostBuilder(args).Build();

        await Parser.Default.ParseArguments<ConsoleOptions>(args)
            .WithParsedAsync(async options => await GetService<ISwaggerMerger>().RunAsync(options.ConfigPath));
    }

    private static T GetService<T>()
        where T : notnull
    {
        return Host.Services.GetRequiredService<T>();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) => builder.SetBasePath(Directory.GetCurrentDirectory()))
            .ConfigureServices((context, services) =>
            {
                services.AddLogging();
                services.AddTransient<ISwaggerDocumentHandler, SwaggerDocumentHandler>();
                services.AddTransient<ISwaggerMergeConfigurationFileHandler, SwaggerMergeConfigurationFileHandler>();
                services.AddTransient<ISwaggerMergeHandler, SwaggerMergeHandler>();
                services.AddTransient<ISwaggerMerger, SwaggerMerger>();
            });

        return hostBuilder;
    }
}