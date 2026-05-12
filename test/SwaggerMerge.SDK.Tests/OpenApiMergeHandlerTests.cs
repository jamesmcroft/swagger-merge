namespace SwaggerMerge.SDK.Tests;

using SwaggerMerge.Common.Configuration.Input;
using SwaggerMerge.V3;
using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Configuration.Output;
using SwaggerMerge.V3.Document;
using Xunit;

public class OpenApiMergeHandlerTests
{
    private readonly OpenApiMergeHandler _handler = new();

    [Fact]
    public void Merge_MultipleDocuments_CombinesPathsFromAllInputs()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet },
                new OpenApiInputConfiguration { File = store }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Merged API", Version = "2.0" },
                Servers = new List<OpenApiServer>
                {
                    new() { Url = "https://api.example.com", Description = "Production" }
                }
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Merged API", result.Info.Title);
        Assert.Equal("2.0", result.Info.Version);
        Assert.NotNull(result.Paths);
        Assert.Contains("/pet", result.Paths.Keys);
        Assert.Contains("/store/inventory", result.Paths.Keys);
        Assert.Contains("/store/order", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithAppendTitle_ConcatenatesInputTitles()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet },
                new OpenApiInputConfiguration
                {
                    File = store,
                    Info = new InputInfoConfiguration { Append = true, Title = "+ Store" }
                }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Pet API", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Pet API + Store", result.Info.Title);
    }

    [Fact]
    public void Merge_WithAppendTitleNoOverride_UsesInputDocumentTitle()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet },
                new OpenApiInputConfiguration
                {
                    File = store,
                    Info = new InputInfoConfiguration { Append = true }
                }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Base", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Base" + store.Info.Title, result.Info.Title);
    }

    [Fact]
    public void Merge_WithStripStartPath_RemovesPrefixFromPaths()
    {
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration
                {
                    File = store,
                    Path = new InputPathConfiguration { StripStart = "/store" }
                }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/inventory", result.Paths.Keys);
        Assert.DoesNotContain("/store/inventory", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithPrependPath_AddsPrefixToPaths()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration
                {
                    File = pet,
                    Path = new InputPathConfiguration { Prepend = "/api" }
                }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/api/pet", result.Paths.Keys);
        Assert.Contains("/api/pet/{petId}", result.Paths.Keys);
        Assert.DoesNotContain("/pet", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithStripStartAndPrepend_TransformsPathsCorrectly()
    {
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration
                {
                    File = store,
                    Path = new InputPathConfiguration { StripStart = "/store", Prepend = "/api/v2" }
                }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/api/v2/inventory", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithComponentSchemas_MergesSchemasFromAllInputs()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet },
                new OpenApiInputConfiguration { File = store }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Components);
        Assert.NotNull(result.Components.Schemas);
        Assert.Contains("Pet", result.Components.Schemas.Keys);
        Assert.Contains("Order", result.Components.Schemas.Keys);
    }

    [Fact]
    public void Merge_SetsOutputServers()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Servers = new List<OpenApiServer>
                {
                    new() { Url = "https://api.example.com", Description = "Production" }
                }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Servers);
        Assert.Single(result.Servers);
        Assert.Equal("https://api.example.com", result.Servers[0].Url);
    }

    [Fact]
    public void Merge_WithNoOutputVersion_DefaultsTo1Point0()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test" }
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("1.0", result.Info.Version);
    }

    [Fact]
    public void Merge_WithNullInputFile_SkipsInput()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json");

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = null },
                new OpenApiInputConfiguration { File = pet }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/pet", result.Paths.Keys);
    }

    [Fact]
    public void Merge_InfersHighestOpenApiVersionFromInputs()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json"); // 3.0.3
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json"); // 3.0.3

        // Modify one to simulate a higher version
        var higherVersionDoc = TestDocumentLoader.LoadOpenApi("pet.openapi.json");
        higherVersionDoc.OpenApiVersion = "3.1.0";

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = store },
                new OpenApiInputConfiguration { File = higherVersionDoc }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("3.1.0", result.OpenApiVersion);
    }

    [Fact]
    public void Merge_AllInputsSameVersion_UsesInputVersion()
    {
        var pet = TestDocumentLoader.LoadOpenApi("pet.openapi.json"); // 3.0.3
        var store = TestDocumentLoader.LoadOpenApi("store.openapi.json"); // 3.0.3

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = pet },
                new OpenApiInputConfiguration { File = store }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Test", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("3.0.3", result.OpenApiVersion);
    }

    // --- Extension Property Merge Tests ---

    [Fact]
    public void Merge_PreservesExtensionPropertiesOnOperations()
    {
        var jsonHandler = new OpenApiJsonDocumentFormatHandler();
        var json = """
        {
          "openapi": "3.0.3",
          "info": { "title": "Ext Test", "version": "1.0" },
          "paths": {
            "/items": {
              "get": {
                "summary": "List items",
                "operationId": "listItems",
                "x-internal": true,
                "x-rate-limit": 100,
                "responses": {
                  "200": { "description": "OK" }
                }
              },
              "post": {
                "summary": "Create item",
                "operationId": "createItem",
                "x-internal": false,
                "responses": {
                  "201": { "description": "Created" }
                }
              }
            }
          }
        }
        """;

        var doc = jsonHandler.Deserialize(json);

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration { File = doc }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Merged", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/items", result.Paths.Keys);

        var getOp = result.Paths["/items"]["get"];
        Assert.NotNull(getOp.JTokenProperties);
        Assert.True(getOp.JTokenProperties.ContainsKey("x-internal"));
        Assert.True(getOp.JTokenProperties["x-internal"].GetBoolean());
        Assert.True(getOp.JTokenProperties.ContainsKey("x-rate-limit"));
        Assert.Equal(100, getOp.JTokenProperties["x-rate-limit"].GetInt32());

        var postOp = result.Paths["/items"]["post"];
        Assert.NotNull(postOp.JTokenProperties);
        Assert.True(postOp.JTokenProperties.ContainsKey("x-internal"));
        Assert.False(postOp.JTokenProperties["x-internal"].GetBoolean());
    }

    [Fact]
    public void Merge_WithOperationExclusion_ExcludesMatchingOperations()
    {
        var jsonHandler = new OpenApiJsonDocumentFormatHandler();
        var json = """
        {
          "openapi": "3.0.3",
          "info": { "title": "Exclusion Test", "version": "1.0" },
          "paths": {
            "/public": {
              "get": {
                "summary": "Public endpoint",
                "operationId": "getPublic",
                "x-internal": false,
                "responses": {
                  "200": { "description": "OK" }
                }
              }
            },
            "/internal": {
              "get": {
                "summary": "Internal endpoint",
                "operationId": "getInternal",
                "x-internal": true,
                "responses": {
                  "200": { "description": "OK" }
                }
              }
            }
          }
        }
        """;

        var doc = jsonHandler.Deserialize(json);

        var exclusion = new InputPathOperationExclusionConfiguration();
        exclusion.Add("x-internal", System.Text.Json.JsonSerializer.SerializeToElement(true));

        var config = new OpenApiMergeConfiguration
        {
            Inputs = new[]
            {
                new OpenApiInputConfiguration
                {
                    File = doc,
                    Path = new InputPathConfiguration
                    {
                        OperationExclusions = exclusion
                    }
                }
            },
            Output = new OpenApiOutputConfiguration
            {
                Info = new OpenApiOutputInfoConfiguration { Title = "Filtered", Version = "1.0" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/public", result.Paths.Keys);
        Assert.DoesNotContain("/internal", result.Paths.Keys);
    }
}
