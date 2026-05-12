namespace SwaggerMerge.SDK.Tests;

using System.Text.Json;
using SwaggerMerge.Document;
using Xunit;

public class SwaggerDocumentHandlerTests
{
    private readonly SwaggerDocumentHandler _handler = new();

    [Fact]
    public async Task LoadFromFilePathAsync_ValidFile_ReturnsDocument()
    {
        var document = await _handler.LoadFromFilePathAsync("Documents/pet.swagger.json");

        Assert.Equal("2.0", document.SwaggerVersion);
        Assert.Equal("Swagger Petstore 2.0", document.Info.Title);
        Assert.Equal("1.0.0", document.Info.Version);
        Assert.NotNull(document.Paths);
        Assert.True(document.Paths.Count > 0);
    }

    [Fact]
    public async Task LoadFromFilePathAsync_TodoFile_ReturnsDocumentWithDefinitions()
    {
        var document = await _handler.LoadFromFilePathAsync("Documents/todo.swagger.json");

        Assert.NotNull(document.Definitions);
        Assert.Contains("Todo", document.Definitions.Keys);
    }

    [Fact]
    public void LoadFromJson_ValidJson_ReturnsDocument()
    {
        var json = File.ReadAllText("Documents/store.swagger.json");
        var document = _handler.LoadFromJson(json);

        Assert.Equal("Swagger Petstore 2.0", document.Info.Title);
        Assert.NotNull(document.Paths);
        Assert.Contains("/store/inventory", document.Paths.Keys);
    }

    [Fact]
    public void LoadFromJson_EmptyObject_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => _handler.LoadFromJson("null"));
    }

    [Fact]
    public async Task SaveToPathAsync_AndReload_RoundTripsDocument()
    {
        var original = await _handler.LoadFromFilePathAsync("Documents/todo.swagger.json");
        var tempPath = Path.Combine(Path.GetTempPath(), $"swagger-test-{Guid.NewGuid()}.json");

        try
        {
            await _handler.SaveToPathAsync(original, tempPath);
            var reloaded = await _handler.LoadFromFilePathAsync(tempPath);

            Assert.Equal(original.Info.Title, reloaded.Info.Title);
            Assert.Equal(original.Info.Version, reloaded.Info.Version);
            Assert.Equal(original.Paths?.Count, reloaded.Paths?.Count);
            Assert.Equal(original.Definitions?.Count, reloaded.Definitions?.Count);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void LoadFromJson_SerializationRoundTrip_PreservesStructure()
    {
        var json = File.ReadAllText("Documents/pet.swagger.json");
        var document = _handler.LoadFromJson(json);
        var reserialized = JsonSerializer.Serialize(document, SwaggerDocumentJsonSerializerContext.Default.SwaggerDocument);
        var roundTripped = _handler.LoadFromJson(reserialized);

        Assert.Equal(document.Info.Title, roundTripped.Info.Title);
        Assert.Equal(document.Paths?.Count, roundTripped.Paths?.Count);
        Assert.Equal(document.Host, roundTripped.Host);
        Assert.Equal(document.BasePath, roundTripped.BasePath);
    }
}
