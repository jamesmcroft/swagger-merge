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
    public void LoadFromJson_NullJson_ThrowsInvalidOperationException()
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

    [Fact]
    public async Task LoadFromFilePathAsync_YamlFile_ReturnsDocument()
    {
        var document = await _handler.LoadFromFilePathAsync("Documents/pet.swagger.yaml");

        Assert.Equal("2.0", document.SwaggerVersion);
        Assert.Equal("Swagger Petstore 2.0", document.Info.Title);
        Assert.Equal("1.0.0", document.Info.Version);
        Assert.NotNull(document.Paths);
        Assert.True(document.Paths.Count > 0);
        Assert.Contains("/pet", document.Paths.Keys);
    }

    [Fact]
    public async Task LoadFromFilePathAsync_YamlStoreFile_ReturnsDocumentWithPaths()
    {
        var document = await _handler.LoadFromFilePathAsync("Documents/store.swagger.yaml");

        Assert.Equal("Swagger Petstore 2.0", document.Info.Title);
        Assert.NotNull(document.Paths);
        Assert.Contains("/store/inventory", document.Paths.Keys);
        Assert.Contains("/store/order", document.Paths.Keys);
    }

    [Fact]
    public void LoadFromYaml_ValidYaml_ReturnsDocument()
    {
        var yaml = File.ReadAllText("Documents/pet.swagger.yaml");
        var document = _handler.LoadFromYaml(yaml);

        Assert.Equal("Swagger Petstore 2.0", document.Info.Title);
        Assert.NotNull(document.Paths);
        Assert.Contains("/pet", document.Paths.Keys);
    }

    [Fact]
    public async Task SaveToPathAsync_YamlExtension_SavesAsYaml()
    {
        var original = await _handler.LoadFromFilePathAsync("Documents/pet.swagger.json");
        var tempPath = Path.Combine(Path.GetTempPath(), $"swagger-test-{Guid.NewGuid()}.yaml");

        try
        {
            await _handler.SaveToPathAsync(original, tempPath);
            var content = await File.ReadAllTextAsync(tempPath);

            Assert.StartsWith("swagger:", content.TrimStart());
            Assert.Contains("Swagger Petstore 2.0", content);
            Assert.DoesNotContain("\"swagger\":", content);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task SaveToPathAsync_ExplicitYamlFormat_SavesAsYaml()
    {
        var original = await _handler.LoadFromFilePathAsync("Documents/pet.swagger.json");
        var tempPath = Path.Combine(Path.GetTempPath(), $"swagger-test-{Guid.NewGuid()}.txt");

        try
        {
            await _handler.SaveToPathAsync(original, tempPath, DocumentFormat.Yaml);
            var content = await File.ReadAllTextAsync(tempPath);

            Assert.StartsWith("swagger:", content.TrimStart());
            Assert.DoesNotContain("\"swagger\":", content);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task YamlRoundTrip_LoadYamlSaveYamlReload_PreservesStructure()
    {
        var original = await _handler.LoadFromFilePathAsync("Documents/pet.swagger.yaml");
        var tempPath = Path.Combine(Path.GetTempPath(), $"swagger-test-{Guid.NewGuid()}.yaml");

        try
        {
            await _handler.SaveToPathAsync(original, tempPath);
            var reloaded = await _handler.LoadFromFilePathAsync(tempPath);

            Assert.Equal(original.Info.Title, reloaded.Info.Title);
            Assert.Equal(original.Info.Version, reloaded.Info.Version);
            Assert.Equal(original.Paths?.Count, reloaded.Paths?.Count);
            Assert.Equal(original.Host, reloaded.Host);
            Assert.Equal(original.BasePath, reloaded.BasePath);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task CrossFormat_LoadJsonSaveYamlReloadFromYaml_PreservesStructure()
    {
        var original = await _handler.LoadFromFilePathAsync("Documents/store.swagger.json");
        var tempYaml = Path.Combine(Path.GetTempPath(), $"swagger-test-{Guid.NewGuid()}.yaml");

        try
        {
            await _handler.SaveToPathAsync(original, tempYaml);
            var reloaded = await _handler.LoadFromFilePathAsync(tempYaml);

            Assert.Equal(original.Info.Title, reloaded.Info.Title);
            Assert.Equal(original.Info.Version, reloaded.Info.Version);
            Assert.Equal(original.Paths?.Count, reloaded.Paths?.Count);
        }
        finally
        {
            File.Delete(tempYaml);
        }
    }

    [Fact]
    public async Task CrossFormat_LoadYamlSaveJsonReloadFromJson_PreservesStructure()
    {
        var original = await _handler.LoadFromFilePathAsync("Documents/pet.swagger.yaml");
        var tempJson = Path.Combine(Path.GetTempPath(), $"swagger-test-{Guid.NewGuid()}.json");

        try
        {
            await _handler.SaveToPathAsync(original, tempJson);
            var reloaded = await _handler.LoadFromFilePathAsync(tempJson);

            Assert.Equal(original.Info.Title, reloaded.Info.Title);
            Assert.Equal(original.Info.Version, reloaded.Info.Version);
            Assert.Equal(original.Paths?.Count, reloaded.Paths?.Count);
        }
        finally
        {
            File.Delete(tempJson);
        }
    }
}
