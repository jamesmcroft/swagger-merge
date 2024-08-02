using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SwaggerMerge.Configuration;
using SwaggerMerge.Configuration.Input;
using SwaggerMerge.Configuration.Output;
using SwaggerMerge.Document;

namespace SwaggerMerge.AotCompatibility.TestApp;

internal sealed class SwaggerMergeHandlerAotTest
{
    [UnconditionalSuppressMessage("", "IL2026", Justification = "Property presence guaranteed by explicit hints.")]
    public static void Test()
    {
        var merger = new SwaggerMergeHandler();

        GuaranteeProperties<SwaggerMergeConfiguration>();

        var d = merger.Merge(GetSwaggerMergeConfiguration());

        // Save the merged document to a file for manual inspection
        File.WriteAllText("Documents/merged.swagger.json", JsonSerializer.Serialize(d, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument));

        Assert(d.Info is { Title: "Swagger Merged" }, "Title was not set.");
        Assert(d.Info is { Version: "1.0.0" }, "Version was not set.");
        Assert(d.Host == "localhost", "Host was not set.");
        Assert(d.BasePath == "/api/", "BasePath was not set.");
        Assert(d.Paths is { Count: > 0 }, "No paths were merged.");
    }

    private static SwaggerMergeConfiguration GetSwaggerMergeConfiguration()
    {
        var documents = Directory.EnumerateFiles("Documents", "*.swagger.json")
            .Select(File.ReadAllText)
            .Select(x => JsonSerializer.Deserialize(x, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument))
            .OfType<SwaggerDocument>()
            .ToList();

        return new SwaggerMergeConfiguration
        {
            Inputs = documents.Select(d => new SwaggerInputConfiguration { File = d }),
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Swagger Merged", Version = "1.0.0" },
                Host = "localhost",
                BasePath = "/api/"
            }
        };
    }

    private static void GuaranteeProperties<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
