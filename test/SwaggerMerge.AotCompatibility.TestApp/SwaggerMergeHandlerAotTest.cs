using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using SwaggerMerge.Configuration;
using SwaggerMerge.Configuration.Input;
using SwaggerMerge.Configuration.Output;
using SwaggerMerge.Document;

namespace SwaggerMerge.AotCompatibility.TestApp;

internal class SwaggerMergeHandlerAotTest
{
    [UnconditionalSuppressMessage("", "IL2026", Justification = "Property presence guaranteed by explicit hints.")]
    public static void Test()
    {
        var merger = new SwaggerMergeHandler();

        GuaranteeProperties<SwaggerMergeConfiguration>();

        var d = merger.Merge(GetSwaggerMergeConfiguration());

        Assert(d.Info is { Title: "Swagger Merged" }, "Title was not set.");
        Assert(d.Paths is { Count: > 0 }, "No paths were merged.");
    }

    private static SwaggerMergeConfiguration GetSwaggerMergeConfiguration()
    {
        var documents = Directory.EnumerateFiles("Documents", "*.swagger.json")
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<SwaggerDocument>)
            .OfType<SwaggerDocument>()
            .ToList();

        return new SwaggerMergeConfiguration
        {
            Inputs = documents.Select(d => new SwaggerInputConfiguration { File = d }),
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Swagger Merged" },
                Host = "localhost"
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
