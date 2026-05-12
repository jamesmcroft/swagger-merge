namespace SwaggerMerge.SDK.Tests;

using System.Text.Json;
using SwaggerMerge.Common.Document;
using SwaggerMerge.V3.Document;
using Xunit;

public class OpenApiDocumentHandlerTests
{
    private readonly OpenApiJsonDocumentFormatHandler _jsonHandler = new();
    private readonly OpenApiYamlDocumentFormatHandler _yamlHandler = new();

    // --- JSON Load Tests ---

    [Fact]
    public void JsonDeserialize_PetOpenApi_ReturnsDocument()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var document = _jsonHandler.Deserialize(json);

        Assert.Equal("3.0.3", document.OpenApiVersion);
        Assert.Equal("Petstore API", document.Info.Title);
        Assert.Equal("1.0.0", document.Info.Version);
        Assert.NotNull(document.Servers);
        Assert.Single(document.Servers);
        Assert.Equal("https://petstore.swagger.io/v3", document.Servers[0].Url);
        Assert.NotNull(document.Paths);
        Assert.True(document.Paths.Count > 0);
        Assert.Contains("/pet", document.Paths.Keys);
    }

    [Fact]
    public void JsonDeserialize_StoreOpenApi_ReturnsDocumentWithComponents()
    {
        var json = File.ReadAllText("Documents/store.openapi.json");
        var document = _jsonHandler.Deserialize(json);

        Assert.NotNull(document.Components);
        Assert.NotNull(document.Components.Schemas);
        Assert.Contains("Order", document.Components.Schemas.Keys);
        Assert.NotNull(document.Components.SecuritySchemes);
        Assert.Contains("api_key", document.Components.SecuritySchemes.Keys);
    }

    [Fact]
    public void JsonDeserialize_PetOpenApi_HasRequestBody()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var document = _jsonHandler.Deserialize(json);

        Assert.NotNull(document.Paths);
        var petPath = document.Paths["/pet"];
        Assert.True(petPath.ContainsKey("post"));

        var postOp = petPath["post"];
        Assert.NotNull(postOp.RequestBody);
        Assert.True(postOp.RequestBody.Required);
        Assert.NotNull(postOp.RequestBody.Content);
        Assert.Contains("application/json", postOp.RequestBody.Content.Keys);
    }

    [Fact]
    public void JsonDeserialize_PetOpenApi_HasTags()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var document = _jsonHandler.Deserialize(json);

        Assert.NotNull(document.Tags);
        Assert.Single(document.Tags);
        Assert.Equal("pet", document.Tags[0].Name);
        Assert.NotNull(document.Tags[0].ExternalDocs);
        Assert.Equal("http://swagger.io", document.Tags[0].ExternalDocs!.Url);
    }

    [Fact]
    public void JsonDeserialize_PetOpenApi_HasSecurity()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var document = _jsonHandler.Deserialize(json);

        Assert.NotNull(document.Security);
        Assert.Single(document.Security);
        Assert.Contains("api_key", document.Security[0].Keys);
    }

    [Fact]
    public void JsonDeserialize_NullJson_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _jsonHandler.Deserialize("null"));
    }

    // --- JSON Round-Trip ---

    [Fact]
    public void JsonRoundTrip_PetOpenApi_PreservesStructure()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var document = _jsonHandler.Deserialize(json);
        var reserialized = _jsonHandler.Serialize(document);
        var roundTripped = _jsonHandler.Deserialize(reserialized);

        Assert.Equal(document.OpenApiVersion, roundTripped.OpenApiVersion);
        Assert.Equal(document.Info.Title, roundTripped.Info.Title);
        Assert.Equal(document.Info.Version, roundTripped.Info.Version);
        Assert.Equal(document.Paths?.Count, roundTripped.Paths?.Count);
        Assert.Equal(document.Servers?.Count, roundTripped.Servers?.Count);
    }

    [Fact]
    public void JsonRoundTrip_StoreOpenApi_PreservesComponents()
    {
        var json = File.ReadAllText("Documents/store.openapi.json");
        var document = _jsonHandler.Deserialize(json);
        var reserialized = _jsonHandler.Serialize(document);
        var roundTripped = _jsonHandler.Deserialize(reserialized);

        Assert.Equal(document.Components?.Schemas?.Count, roundTripped.Components?.Schemas?.Count);
        Assert.Equal(document.Components?.SecuritySchemes?.Count, roundTripped.Components?.SecuritySchemes?.Count);
    }

    // --- YAML Load Tests ---

    [Fact]
    public void YamlDeserialize_PetOpenApi_ReturnsDocument()
    {
        var yaml = File.ReadAllText("Documents/pet.openapi.yaml");
        var document = _yamlHandler.Deserialize(yaml);

        Assert.Equal("3.0.3", document.OpenApiVersion);
        Assert.Equal("Petstore API", document.Info.Title);
        Assert.Equal("1.0.0", document.Info.Version);
        Assert.NotNull(document.Servers);
        Assert.Single(document.Servers);
        Assert.NotNull(document.Paths);
        Assert.Contains("/pet", document.Paths.Keys);
    }

    [Fact]
    public void YamlDeserialize_StoreOpenApi_ReturnsDocumentWithComponents()
    {
        var yaml = File.ReadAllText("Documents/store.openapi.yaml");
        var document = _yamlHandler.Deserialize(yaml);

        Assert.NotNull(document.Components);
        Assert.NotNull(document.Components.Schemas);
        Assert.Contains("Order", document.Components.Schemas.Keys);
    }

    // --- YAML Round-Trip ---

    [Fact]
    public void YamlRoundTrip_PetOpenApi_PreservesStructure()
    {
        var yaml = File.ReadAllText("Documents/pet.openapi.yaml");
        var document = _yamlHandler.Deserialize(yaml);
        var reserialized = _yamlHandler.Serialize(document);
        var roundTripped = _yamlHandler.Deserialize(reserialized);

        Assert.Equal(document.OpenApiVersion, roundTripped.OpenApiVersion);
        Assert.Equal(document.Info.Title, roundTripped.Info.Title);
        Assert.Equal(document.Paths?.Count, roundTripped.Paths?.Count);
    }

    // --- Cross-Format Tests ---

    [Fact]
    public void CrossFormat_JsonToYaml_PreservesDocument()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var document = _jsonHandler.Deserialize(json);
        var yamlOutput = _yamlHandler.Serialize(document);
        var fromYaml = _yamlHandler.Deserialize(yamlOutput);

        Assert.Equal(document.OpenApiVersion, fromYaml.OpenApiVersion);
        Assert.Equal(document.Info.Title, fromYaml.Info.Title);
        Assert.Equal(document.Paths?.Count, fromYaml.Paths?.Count);
        Assert.Equal(document.Components?.Schemas?.Count, fromYaml.Components?.Schemas?.Count);
    }

    [Fact]
    public void CrossFormat_YamlToJson_PreservesDocument()
    {
        var yaml = File.ReadAllText("Documents/pet.openapi.yaml");
        var document = _yamlHandler.Deserialize(yaml);
        var jsonOutput = _jsonHandler.Serialize(document);
        var fromJson = _jsonHandler.Deserialize(jsonOutput);

        Assert.Equal(document.OpenApiVersion, fromJson.OpenApiVersion);
        Assert.Equal(document.Info.Title, fromJson.Info.Title);
        Assert.Equal(document.Paths?.Count, fromJson.Paths?.Count);
    }

    // --- Spec Version Detection Tests ---

    [Fact]
    public void DetectVersion_SwaggerV2Json_ReturnsSwaggerV2()
    {
        var json = File.ReadAllText("Documents/pet.swagger.json");
        var version = SpecVersionDetector.DetectVersion(json);
        Assert.Equal(SpecVersion.SwaggerV2, version);
    }

    [Fact]
    public void DetectVersion_OpenApiV3Json_ReturnsOpenApiV3()
    {
        var json = File.ReadAllText("Documents/pet.openapi.json");
        var version = SpecVersionDetector.DetectVersion(json);
        Assert.Equal(SpecVersion.OpenApiV3, version);
    }

    [Fact]
    public void DetectVersion_SwaggerV2Yaml_ReturnsSwaggerV2()
    {
        var yaml = File.ReadAllText("Documents/pet.swagger.yaml");
        var version = SpecVersionDetector.DetectVersion(yaml);
        Assert.Equal(SpecVersion.SwaggerV2, version);
    }

    [Fact]
    public void DetectVersion_OpenApiV3Yaml_ReturnsOpenApiV3()
    {
        var yaml = File.ReadAllText("Documents/pet.openapi.yaml");
        var version = SpecVersionDetector.DetectVersion(yaml);
        Assert.Equal(SpecVersion.OpenApiV3, version);
    }

    [Fact]
    public void DetectVersion_EmptyContent_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => SpecVersionDetector.DetectVersion(""));
    }

    [Fact]
    public void DetectVersion_UnrecognizedContent_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => SpecVersionDetector.DetectVersion("just some random text"));
    }

    [Fact]
    public void DetectVersion_InlineSwaggerV2_ReturnsSwaggerV2()
    {
        var version = SpecVersionDetector.DetectVersion("{\"swagger\": \"2.0\", \"info\": {}}");
        Assert.Equal(SpecVersion.SwaggerV2, version);
    }

    [Fact]
    public void DetectVersion_InlineOpenApiV3_ReturnsOpenApiV3()
    {
        var version = SpecVersionDetector.DetectVersion("{\"openapi\": \"3.0.3\", \"info\": {}}");
        Assert.Equal(SpecVersion.OpenApiV3, version);
    }

    [Fact]
    public void DetectVersion_YamlInlineSwaggerV2_ReturnsSwaggerV2()
    {
        var version = SpecVersionDetector.DetectVersion("swagger: '2.0'\ninfo:\n  title: test");
        Assert.Equal(SpecVersion.SwaggerV2, version);
    }

    [Fact]
    public void DetectVersion_YamlInlineOpenApiV3_ReturnsOpenApiV3()
    {
        var version = SpecVersionDetector.DetectVersion("openapi: '3.0.3'\ninfo:\n  title: test");
        Assert.Equal(SpecVersion.OpenApiV3, version);
    }
}
