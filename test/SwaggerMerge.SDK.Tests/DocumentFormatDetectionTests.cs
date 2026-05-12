namespace SwaggerMerge.SDK.Tests;

using SwaggerMerge.Document;
using Xunit;

public class DocumentFormatDetectionTests
{
    [Theory]
    [InlineData("file.json", DocumentFormat.Json)]
    [InlineData("file.yaml", DocumentFormat.Yaml)]
    [InlineData("file.yml", DocumentFormat.Yaml)]
    [InlineData("path/to/file.JSON", DocumentFormat.Json)]
    [InlineData("path/to/file.YAML", DocumentFormat.Yaml)]
    [InlineData("path/to/file.YML", DocumentFormat.Yaml)]
    public void DetectFormat_ByExtension_ReturnsCorrectFormat(string filePath, DocumentFormat expected)
    {
        var result = ISwaggerDocumentHandler.DetectFormat(filePath);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DetectFormat_UnknownExtension_WithJsonContent_ReturnsJson()
    {
        var result = ISwaggerDocumentHandler.DetectFormat("file.txt", "{ \"swagger\": \"2.0\" }");
        Assert.Equal(DocumentFormat.Json, result);
    }

    [Fact]
    public void DetectFormat_UnknownExtension_WithYamlContent_ReturnsYaml()
    {
        var result = ISwaggerDocumentHandler.DetectFormat("file.txt", "swagger: '2.0'");
        Assert.Equal(DocumentFormat.Yaml, result);
    }

    [Fact]
    public void DetectFormat_UnknownExtension_NoContent_DefaultsToJson()
    {
        var result = ISwaggerDocumentHandler.DetectFormat("file.txt");
        Assert.Equal(DocumentFormat.Json, result);
    }

    [Fact]
    public void DetectFormat_JsonExtension_IgnoresContent()
    {
        var result = ISwaggerDocumentHandler.DetectFormat("file.json", "swagger: '2.0'");
        Assert.Equal(DocumentFormat.Json, result);
    }

    [Fact]
    public void DetectFormat_YamlExtension_IgnoresContent()
    {
        var result = ISwaggerDocumentHandler.DetectFormat("file.yaml", "{ }");
        Assert.Equal(DocumentFormat.Yaml, result);
    }
}
