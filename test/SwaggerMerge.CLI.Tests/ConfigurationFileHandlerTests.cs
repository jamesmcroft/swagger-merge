namespace SwaggerMerge.CLI.Tests;

using SwaggerMerge.Common.Configuration.Input;
using SwaggerMerge.Common.Document;
using SwaggerMerge.Common.Exceptions;
using SwaggerMerge.Infrastructure.Configuration.Merge;
using SwaggerMerge.Infrastructure.Configuration.Merge.Input;
using SwaggerMerge.Infrastructure.Configuration.Merge.Output;
using SwaggerMerge.V2.Document;
using SwaggerMerge.V3.Document;
using Xunit;

public class ConfigurationFileHandlerTests
{
    private readonly SwaggerMergeConfigurationFileHandler _handler;

    public ConfigurationFileHandlerTests()
    {
        var v2DocHandler = new SwaggerDocumentHandler();
        var v3DocHandler = new OpenApiDocumentHandler();
        _handler = new SwaggerMergeConfigurationFileHandler(v2DocHandler, v3DocHandler);
    }

    [Fact]
    public void Validate_EmptyInputs_ReturnsInvalid()
    {
        var config = new SwaggerMergeConfigurationFile
        {
            Inputs = [],
            Output = new SwaggerOutputConfiguration { File = "output.json" }
        };

        var result = _handler.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors!, e => e.Contains("input"));
    }

    [Fact]
    public void Validate_InputWithEmptyFilePath_ReturnsInvalid()
    {
        var config = new SwaggerMergeConfigurationFile
        {
            Inputs = [new SwaggerInputConfiguration { File = "" }],
            Output = new SwaggerOutputConfiguration { File = "output.json" }
        };

        var result = _handler.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors!, e => e.Contains("file path"));
    }

    [Fact]
    public void Validate_EmptyOutputFile_ReturnsInvalid()
    {
        var config = new SwaggerMergeConfigurationFile
        {
            Inputs = [new SwaggerInputConfiguration { File = "pet.swagger.json" }],
            Output = new SwaggerOutputConfiguration { File = "" }
        };

        var result = _handler.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors!, e => e.Contains("output"));
    }

    [Fact]
    public void Validate_ValidConfig_ReturnsValid()
    {
        var config = new SwaggerMergeConfigurationFile
        {
            Inputs =
            [
                new SwaggerInputConfiguration { File = "pet.swagger.json" },
                new SwaggerInputConfiguration { File = "store.swagger.json" }
            ],
            Output = new SwaggerOutputConfiguration { File = "merged.json" }
        };

        var result = _handler.Validate(config);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ConvertWithVersionDetection_V2Inputs_ReturnsV2Config()
    {
        var config = CreateConfigForDocuments("pet.swagger.json", "store.swagger.json");

        var result = await _handler.ConvertWithVersionDetectionAsync(config);

        Assert.Equal(SpecVersion.SwaggerV2, result.Version);
        Assert.NotNull(result.V2Config);
        Assert.Null(result.V3Config);
    }

    [Fact]
    public async Task ConvertWithVersionDetection_V3Inputs_ReturnsV3Config()
    {
        var config = CreateConfigForDocuments("pet.openapi.json", "store.openapi.json");

        var result = await _handler.ConvertWithVersionDetectionAsync(config);

        Assert.Equal(SpecVersion.OpenApiV3, result.Version);
        Assert.NotNull(result.V3Config);
        Assert.Null(result.V2Config);
    }

    [Fact]
    public async Task ConvertWithVersionDetection_V3Inputs_MapsServersFromConfig()
    {
        var config = CreateConfigForDocuments("pet.openapi.json", "store.openapi.json");
        config.Output.Servers =
        [
            new OpenApiServer { Url = "https://api.example.com", Description = "Production" }
        ];

        var result = await _handler.ConvertWithVersionDetectionAsync(config);

        Assert.Equal(SpecVersion.OpenApiV3, result.Version);
        Assert.NotNull(result.V3Config);
        Assert.NotNull(result.V3Config.Output.Servers);
        Assert.Single(result.V3Config.Output.Servers);
        Assert.Equal("https://api.example.com", result.V3Config.Output.Servers[0].Url);
    }

    [Fact]
    public async Task ConvertWithVersionDetection_MixedVersionInputs_ThrowsException()
    {
        var config = CreateConfigForDocuments("pet.swagger.json", "store.openapi.json");

        await Assert.ThrowsAsync<SwaggerMergeException>(
            () => _handler.ConvertWithVersionDetectionAsync(config));
    }

    [Fact]
    public async Task ConvertAsync_V2Config_LoadsAllInputDocuments()
    {
        var config = CreateConfigForDocuments("pet.swagger.json", "store.swagger.json");

        var result = await _handler.ConvertAsync(config);

        Assert.Equal(2, result.Inputs.Count());
        Assert.All(result.Inputs, input => Assert.NotNull(input.File));
    }

    [Fact]
    public async Task ConvertAsync_V2Config_PreservesPathConfiguration()
    {
        var config = new SwaggerMergeConfigurationFile
        {
            Inputs =
            [
                new SwaggerInputConfiguration
                {
                    File = Path.Combine("Documents", "pet.swagger.json"),
                    Path = new InputPathConfiguration { StripStart = "/v2", Prepend = "/api" }
                }
            ],
            Output = new SwaggerOutputConfiguration { File = "merged.json" }
        };

        var result = await _handler.ConvertAsync(config);

        var input = result.Inputs.First();
        Assert.NotNull(input.Path);
        Assert.Equal("/v2", input.Path.StripStart);
        Assert.Equal("/api", input.Path.Prepend);
    }

    [Fact]
    public async Task ConvertAsync_V2Config_PreservesOutputConfiguration()
    {
        var config = new SwaggerMergeConfigurationFile
        {
            Inputs =
            [
                new SwaggerInputConfiguration { File = Path.Combine("Documents", "pet.swagger.json") }
            ],
            Output = new SwaggerOutputConfiguration
            {
                File = "merged.json",
                Host = "api.example.com",
                BasePath = "/v1",
                Schemes = ["https"]
            }
        };

        var result = await _handler.ConvertAsync(config);

        Assert.Equal("api.example.com", result.Output.Host);
        Assert.Equal("/v1", result.Output.BasePath);
        Assert.NotNull(result.Output.Schemes);
        Assert.Contains("https", result.Output.Schemes);
    }

    private static SwaggerMergeConfigurationFile CreateConfigForDocuments(params string[] fileNames)
    {
        return new SwaggerMergeConfigurationFile
        {
            Inputs = fileNames.Select(f => new SwaggerInputConfiguration
            {
                File = Path.Combine("Documents", f)
            }).ToList(),
            Output = new SwaggerOutputConfiguration { File = "merged.json" }
        };
    }
}
