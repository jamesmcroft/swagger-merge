namespace SwaggerMerge.SDK.Tests;

using System.Text.Json;
using SwaggerMerge.Document;

internal static class TestDocumentLoader
{
    public static SwaggerDocument Load(string fileName)
    {
        var json = File.ReadAllText(Path.Combine("Documents", fileName));
        return JsonSerializer.Deserialize(json, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument)
               ?? throw new InvalidOperationException($"Failed to deserialize {fileName}");
    }
}
