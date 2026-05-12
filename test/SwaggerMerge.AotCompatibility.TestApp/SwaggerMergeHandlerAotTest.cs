using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SwaggerMerge.V2.Configuration;
using SwaggerMerge.V2.Configuration.Input;
using SwaggerMerge.V2.Configuration.Output;
using SwaggerMerge.V2.Document;
using SwaggerMerge.V3;
using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Configuration.Output;
using SwaggerMerge.V3.Document;
using SwaggerMerge.Common.Document;

namespace SwaggerMerge.AotCompatibility.TestApp;

internal sealed class SwaggerMergeHandlerAotTest
{
    [UnconditionalSuppressMessage("", "IL2026", Justification = "Property presence guaranteed by explicit hints.")]
    public static void Test()
    {
        var merger = new SwaggerMerge.V2.SwaggerMergeHandler();

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

        TestOpenApiV3Merge();
        TestOpenApiV3YamlRoundTrip();
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

    [UnconditionalSuppressMessage("", "IL2026", Justification = "Property presence guaranteed by explicit hints.")]
    private static void TestOpenApiV3Merge()
    {
        var merger = new OpenApiMergeHandler();
        var jsonHandler = new OpenApiJsonDocumentFormatHandler();

        GuaranteeProperties<OpenApiMergeConfiguration>();

        var documents = Directory.EnumerateFiles("Documents", "*.openapi.json")
            .Select(File.ReadAllText)
            .Select(jsonHandler.Deserialize)
            .ToList();

        var config = new OpenApiMergeConfiguration
        {
            Inputs = documents.Select(d => new OpenApiInputConfiguration { File = d }),
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "OpenAPI Merged", Version = "2.0.0" },
                Servers = new List<OpenApiServer>
                {
                    new() { Url = "https://localhost/api", Description = "Local server" }
                }
            }
        };

        var merged = merger.Merge(config);

        File.WriteAllText("Documents/merged.openapi.json",
            JsonSerializer.Serialize(merged, OpenApiDocumentJsonSerializerContext.Default.OpenApiDocument));

        Assert(merged.Info is { Title: "OpenAPI Merged" }, "V3: Title was not set.");
        Assert(merged.Info is { Version: "2.0.0" }, "V3: Version was not set.");
        Assert(merged.Paths is { Count: > 0 }, "V3: No paths were merged.");
        Assert(merged.Servers is { Count: > 0 }, "V3: Servers were not set.");
    }

    [UnconditionalSuppressMessage("", "IL2026", Justification = "YAML conversion uses untyped intermediary objects only.")]
    private static void TestOpenApiV3YamlRoundTrip()
    {
        var handler = new OpenApiDocumentHandler();

        // Load from JSON
        var jsonContent = File.ReadAllText("Documents/pet.openapi.json");
        var docFromJson = handler.LoadFromJson(jsonContent);
        Assert(docFromJson.OpenApiVersion == "3.0.3", "V3 YAML: openapi version was not 3.0.3.");
        Assert(docFromJson.Paths is { Count: > 0 }, "V3 YAML: no paths were loaded.");

        // Save as YAML and reload
        handler.SaveToPathAsync(docFromJson, "Documents/pet.openapi.roundtrip.yaml", DocumentFormat.Yaml).GetAwaiter().GetResult();
        var reloaded = handler.LoadFromFilePathAsync("Documents/pet.openapi.roundtrip.yaml").GetAwaiter().GetResult();
        Assert(reloaded.OpenApiVersion == "3.0.3", "V3 YAML round-trip: openapi version was not 3.0.3.");
        Assert(reloaded.Paths is { Count: > 0 }, "V3 YAML round-trip: no paths after reload.");
        Assert(reloaded.Info.Title == docFromJson.Info.Title, "V3 YAML round-trip: title mismatch.");

        // Cross-format: save YAML-loaded doc as JSON, reload
        handler.SaveToPathAsync(reloaded, "Documents/pet.openapi.fromyaml.json", DocumentFormat.Json).GetAwaiter().GetResult();
        var jsonReloaded = handler.LoadFromFilePathAsync("Documents/pet.openapi.fromyaml.json").GetAwaiter().GetResult();
        Assert(jsonReloaded.Paths is { Count: > 0 }, "V3 Cross-format: no paths after YAML-to-JSON round-trip.");
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
