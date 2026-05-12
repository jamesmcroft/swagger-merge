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

        TestYamlRoundTrip();
    }

    [UnconditionalSuppressMessage("", "IL2026", Justification = "YAML conversion uses untyped intermediary objects only.")]
    private static void TestYamlRoundTrip()
    {
        var handler = new SwaggerDocumentHandler();

        // Load from YAML
        var yamlContent = File.ReadAllText("Documents/pet.swagger.yaml");
        var docFromYaml = handler.LoadFromYaml(yamlContent);
        Assert(docFromYaml.SwaggerVersion == "2.0", "YAML load: swagger version was not 2.0.");
        Assert(docFromYaml.Paths is { Count: > 0 }, "YAML load: no paths were loaded.");

        // Save as YAML and reload
        handler.SaveToPathAsync(docFromYaml, "Documents/pet.roundtrip.yaml", DocumentFormat.Yaml).GetAwaiter().GetResult();
        var reloaded = handler.LoadFromFilePathAsync("Documents/pet.roundtrip.yaml").GetAwaiter().GetResult();
        Assert(reloaded.SwaggerVersion == "2.0", "YAML round-trip: swagger version was not 2.0.");
        Assert(reloaded.Paths is { Count: > 0 }, "YAML round-trip: no paths after reload.");
        Assert(reloaded.Info.Title == docFromYaml.Info.Title, "YAML round-trip: title mismatch.");

        // Cross-format: save YAML-loaded doc as JSON, reload
        handler.SaveToPathAsync(docFromYaml, "Documents/pet.fromyaml.json", DocumentFormat.Json).GetAwaiter().GetResult();
        var jsonReloaded = handler.LoadFromFilePathAsync("Documents/pet.fromyaml.json").GetAwaiter().GetResult();
        Assert(jsonReloaded.Paths is { Count: > 0 }, "Cross-format: no paths after YAML-to-JSON round-trip.");
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
