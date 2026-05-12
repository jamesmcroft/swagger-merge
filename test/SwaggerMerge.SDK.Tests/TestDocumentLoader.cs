namespace SwaggerMerge.SDK.Tests;

using System.Text.Json;
using SwaggerMerge.V2.Document;
using SwaggerMerge.Common.Document;
using SwaggerMerge.V3.Document;

internal static class TestDocumentLoader
{
    private static readonly SwaggerDocumentHandler Handler = new();
    private static readonly OpenApiJsonDocumentFormatHandler OpenApiJsonHandler = new();
    private static readonly OpenApiYamlDocumentFormatHandler OpenApiYamlHandler = new();

    public static SwaggerDocument Load(string fileName)
    {
        var json = File.ReadAllText(Path.Combine("Documents", fileName));
        return JsonSerializer.Deserialize(json, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument)
               ?? throw new InvalidOperationException($"Failed to deserialize {fileName}");
    }

    public static SwaggerDocument LoadYaml(string fileName)
    {
        var yaml = File.ReadAllText(Path.Combine("Documents", fileName));
        return Handler.LoadFromYaml(yaml);
    }

    public static OpenApiDocument LoadOpenApi(string fileName)
    {
        var content = File.ReadAllText(Path.Combine("Documents", fileName));
        return fileName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)
               || fileName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase)
            ? OpenApiYamlHandler.Deserialize(content)
            : OpenApiJsonHandler.Deserialize(content);
    }
}
