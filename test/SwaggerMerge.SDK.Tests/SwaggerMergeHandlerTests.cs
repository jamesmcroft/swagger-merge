namespace SwaggerMerge.SDK.Tests;

using SwaggerMerge.Common.Configuration.Input;
using SwaggerMerge.V2;
using SwaggerMerge.V2.Configuration;
using SwaggerMerge.V2.Configuration.Input;
using SwaggerMerge.V2.Configuration.Output;
using SwaggerMerge.V2.Document;
using Xunit;

public class SwaggerMergeHandlerTests
{
    private readonly SwaggerMergeHandler _handler = new();

    [Fact]
    public void Merge_MultipleDocuments_CombinesPathsFromAllInputs()
    {
        var pet = TestDocumentLoader.Load("pet.swagger.json");
        var store = TestDocumentLoader.Load("store.swagger.json");
        var user = TestDocumentLoader.Load("user.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = pet },
                new SwaggerInputConfiguration { File = store },
                new SwaggerInputConfiguration { File = user }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Merged API", Version = "2.0" },
                Host = "api.example.com",
                BasePath = "/v1"
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Merged API", result.Info.Title);
        Assert.Equal("2.0", result.Info.Version);
        Assert.Equal("api.example.com", result.Host);
        Assert.Equal("/v1", result.BasePath);
        Assert.NotNull(result.Paths);
        Assert.Contains("/pet", result.Paths.Keys);
        Assert.Contains("/store/inventory", result.Paths.Keys);
        Assert.Contains("/user", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithAppendTitle_ConcatenatesInputTitles()
    {
        var pet = TestDocumentLoader.Load("pet.swagger.json");
        var store = TestDocumentLoader.Load("store.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = pet },
                new SwaggerInputConfiguration
                {
                    File = store,
                    Info = new InputInfoConfiguration { Append = true, Title = "+ Store" }
                }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Pet API", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Pet API + Store", result.Info.Title);
    }

    [Fact]
    public void Merge_WithAppendTitleNoOverride_UsesInputDocumentTitle()
    {
        var pet = TestDocumentLoader.Load("pet.swagger.json");
        var store = TestDocumentLoader.Load("store.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = pet },
                new SwaggerInputConfiguration
                {
                    File = store,
                    Info = new InputInfoConfiguration { Append = true }
                }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Base", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Base" + store.Info.Title, result.Info.Title);
    }

    [Fact]
    public void Merge_WithStripStartPath_RemovesPrefixFromPaths()
    {
        var store = TestDocumentLoader.Load("store.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration
                {
                    File = store,
                    Path = new InputPathConfiguration { StripStart = "/store" }
                }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Host = "localhost"
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
        var todo = TestDocumentLoader.Load("todo.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration
                {
                    File = todo,
                    Path = new InputPathConfiguration { Prepend = "/api" }
                }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/api/todos", result.Paths.Keys);
        Assert.Contains("/api/todos/{id}", result.Paths.Keys);
        Assert.DoesNotContain("/todos", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithStripStartAndPrepend_TransformsPathsCorrectly()
    {
        var store = TestDocumentLoader.Load("store.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration
                {
                    File = store,
                    Path = new InputPathConfiguration { StripStart = "/store", Prepend = "/api/v2" }
                }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/api/v2/inventory", result.Paths.Keys);
    }

    [Fact]
    public void Merge_WithDefinitions_MergesDefinitionsFromAllInputs()
    {
        var todo = TestDocumentLoader.Load("todo.swagger.json");
        var pet = TestDocumentLoader.Load("pet.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = todo },
                new SwaggerInputConfiguration { File = pet }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Definitions);
        Assert.Contains("Todo", result.Definitions.Keys);
    }

    [Fact]
    public void Merge_SetsOutputSchemes()
    {
        var pet = TestDocumentLoader.Load("pet.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = pet }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Host = "localhost",
                Schemes = new List<string> { "https" }
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Schemes);
        Assert.Single(result.Schemes);
        Assert.Equal("https", result.Schemes[0]);
    }

    [Fact]
    public void Merge_WithNoOutputVersion_DefaultsTo1Point0()
    {
        var pet = TestDocumentLoader.Load("pet.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = pet }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("1.0", result.Info.Version);
    }

    [Fact]
    public void Merge_WithNullInputFile_SkipsInput()
    {
        var pet = TestDocumentLoader.Load("pet.swagger.json");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = null },
                new SwaggerInputConfiguration { File = pet }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Test", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.NotNull(result.Paths);
        Assert.Contains("/pet", result.Paths.Keys);
    }

    [Fact]
    public void Merge_MixedFormatInputs_JsonAndYaml_CombinesPaths()
    {
        var jsonDoc = TestDocumentLoader.Load("pet.swagger.json");
        var yamlDoc = TestDocumentLoader.LoadYaml("store.swagger.yaml");

        var config = new SwaggerMergeConfiguration
        {
            Inputs = new[]
            {
                new SwaggerInputConfiguration { File = jsonDoc },
                new SwaggerInputConfiguration { File = yamlDoc }
            },
            Output = new SwaggerOutputConfiguration
            {
                Info = new SwaggerOutputInfoConfiguration { Title = "Mixed Format API", Version = "1.0" },
                Host = "localhost"
            }
        };

        var result = _handler.Merge(config);

        Assert.Equal("Mixed Format API", result.Info.Title);
        Assert.NotNull(result.Paths);
        Assert.Contains("/pet", result.Paths.Keys);
        Assert.Contains("/store/inventory", result.Paths.Keys);
    }
}
