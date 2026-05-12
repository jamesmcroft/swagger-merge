namespace SwaggerMerge.Infrastructure.Configuration.Merge;

using System.Text;
using System.Text.Json;
using SwaggerMerge.V2.Document;
using SwaggerMerge.V3.Document;
using SwaggerMerge.Common.Document;
using SwaggerMerge.Common.Exceptions;
using SwaggerMerge.V2.Configuration;
using SwaggerMerge.V2.Configuration.Input;
using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Configuration.Output;

internal sealed class SwaggerMergeConfigurationFileHandler(
    ISwaggerDocumentHandler v2DocumentHandler,
    IOpenApiDocumentHandler v3DocumentHandler)
    : ISwaggerMergeConfigurationFileHandler
{
    public async Task<SwaggerMergeConfigurationFile> LoadAsync(string configFilePath)
    {
        var content = await ReadAllTextAsync(configFilePath);
        var config = JsonSerializer.Deserialize<SwaggerMergeConfigurationFile>(content) ??
                     throw new SwaggerMergeException(
                         $"The Swagger merge configuration file at '{configFilePath}' could not be loaded correctly as it is not in the correct format.");

        var configDirectory = Path.GetDirectoryName(configFilePath);
        if (configDirectory != null)
        {
            Directory.SetCurrentDirectory(configDirectory);
        }

        return config;
    }

    public SwaggerMergeConfigurationFileValidationResult Validate(SwaggerMergeConfigurationFile config)
    {
        var isValid = true;
        var errors = new List<string>();

        if (!config.Inputs.Any())
        {
            isValid = false;
            errors.Add("At least one input must be defined");
        }

        if (config.Inputs.Any(input => string.IsNullOrWhiteSpace(input.File)))
        {
            isValid = false;
            errors.Add("All inputs must have a file path defined");
        }

        if (string.IsNullOrWhiteSpace(config.Output.File))
        {
            isValid = false;
            errors.Add("The output file must be defined");
        }

        return new SwaggerMergeConfigurationFileValidationResult(isValid, errors);
    }

    public async Task<SwaggerMergeConfiguration> ConvertAsync(SwaggerMergeConfigurationFile config)
    {
        var inputs = new List<SwaggerInputConfiguration>();

        foreach (var input in config.Inputs)
        {
            var inputFile = await v2DocumentHandler.LoadFromFilePathAsync(input.File);
            inputs.Add(new SwaggerInputConfiguration { File = inputFile, Path = input.Path, Info = input.Info });
        }

        return new SwaggerMergeConfiguration { Inputs = inputs, Output = config.Output };
    }

    public async Task<MergeConfigResult> ConvertWithVersionDetectionAsync(SwaggerMergeConfigurationFile config)
    {
        var inputFiles = config.Inputs.ToList();
        if (inputFiles.Count == 0)
        {
            throw new SwaggerMergeException("At least one input file is required.");
        }

        // Detect version from the first input file
        var firstContent = await ReadAllTextAsync(inputFiles[0].File);

        SpecVersion version;
        try
        {
            version = SpecVersionDetector.DetectVersion(firstContent);
        }
        catch (InvalidOperationException ex)
        {
            throw new SwaggerMergeException(
                $"Could not determine the specification version of '{inputFiles[0].File}'. {ex.Message}", ex);
        }

        return version switch
        {
            SpecVersion.SwaggerV2 => await ConvertV2Async(config, inputFiles, firstContent),
            SpecVersion.OpenApiV3 => await ConvertV3Async(config, inputFiles, firstContent),
            _ => throw new SwaggerMergeException($"Unsupported specification version: {version}")
        };
    }

    private async Task<MergeConfigResult> ConvertV2Async(
        SwaggerMergeConfigurationFile config,
        List<Input.SwaggerInputConfiguration> inputFiles,
        string firstContent)
    {
        var inputs = new List<SwaggerInputConfiguration>();
        var format = ISwaggerDocumentHandler.DetectFormat(inputFiles[0].File, firstContent);
        var firstDoc = format == DocumentFormat.Yaml
            ? v2DocumentHandler.LoadFromYaml(firstContent)
            : v2DocumentHandler.LoadFromJson(firstContent);
        inputs.Add(new SwaggerInputConfiguration { File = firstDoc, Path = inputFiles[0].Path, Info = inputFiles[0].Info });

        for (var i = 1; i < inputFiles.Count; i++)
        {
            var content = await ReadAllTextAsync(inputFiles[i].File);

            try
            {
                var inputVersion = SpecVersionDetector.DetectVersion(content);
                if (inputVersion != SpecVersion.SwaggerV2)
                {
                    throw new SwaggerMergeException(
                        $"Input file '{inputFiles[i].File}' is {inputVersion} but the first input is Swagger V2. All inputs must be the same specification version.");
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new SwaggerMergeException(
                    $"Could not determine the specification version of '{inputFiles[i].File}'. {ex.Message}", ex);
            }

            var inputFormat = ISwaggerDocumentHandler.DetectFormat(inputFiles[i].File, content);
            var doc = inputFormat == DocumentFormat.Yaml
                ? v2DocumentHandler.LoadFromYaml(content)
                : v2DocumentHandler.LoadFromJson(content);
            inputs.Add(new SwaggerInputConfiguration { File = doc, Path = inputFiles[i].Path, Info = inputFiles[i].Info });
        }

        return new MergeConfigResult
        {
            Version = SpecVersion.SwaggerV2,
            V2Config = new SwaggerMergeConfiguration { Inputs = inputs, Output = config.Output }
        };
    }

    private async Task<MergeConfigResult> ConvertV3Async(
        SwaggerMergeConfigurationFile config,
        List<Input.SwaggerInputConfiguration> inputFiles,
        string firstContent)
    {
        var inputs = new List<OpenApiInputConfiguration>();
        var format = IOpenApiDocumentHandler.DetectFormat(inputFiles[0].File, firstContent);
        var firstDoc = format == DocumentFormat.Yaml
            ? v3DocumentHandler.LoadFromYaml(firstContent)
            : v3DocumentHandler.LoadFromJson(firstContent);
        inputs.Add(new OpenApiInputConfiguration { File = firstDoc, Path = inputFiles[0].Path, Info = inputFiles[0].Info });

        for (var i = 1; i < inputFiles.Count; i++)
        {
            var content = await ReadAllTextAsync(inputFiles[i].File);

            try
            {
                var inputVersion = SpecVersionDetector.DetectVersion(content);
                if (inputVersion != SpecVersion.OpenApiV3)
                {
                    throw new SwaggerMergeException(
                        $"Input file '{inputFiles[i].File}' is {inputVersion} but the first input is OpenAPI V3. All inputs must be the same specification version.");
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new SwaggerMergeException(
                    $"Could not determine the specification version of '{inputFiles[i].File}'. {ex.Message}", ex);
            }

            var inputFormat = IOpenApiDocumentHandler.DetectFormat(inputFiles[i].File, content);
            var doc = inputFormat == DocumentFormat.Yaml
                ? v3DocumentHandler.LoadFromYaml(content)
                : v3DocumentHandler.LoadFromJson(content);
            inputs.Add(new OpenApiInputConfiguration { File = doc, Path = inputFiles[i].Path, Info = inputFiles[i].Info });
        }

        var v3Output = new OpenApiOutputConfiguration
        {
            Info = config.Output.Info != null
                ? new OpenApiOutputInfoConfiguration
                {
                    Title = config.Output.Info.Title,
                    Version = config.Output.Info.Version
                }
                : null,
            Servers = config.Output.Servers,
            SecuritySchemes = config.Output.SecurityDefinitions != null
                ? ConvertV2SecurityDefinitionsToV3(config.Output.SecurityDefinitions)
                : null,
            Security = config.Output.Security?.Select(ConvertV2SecurityRequirementToV3).ToList()
        };

        return new MergeConfigResult
        {
            Version = SpecVersion.OpenApiV3,
            V3Config = new OpenApiMergeConfiguration { Inputs = inputs, Output = v3Output }
        };
    }

    private static OpenApiDocumentSecurityDefinitions ConvertV2SecurityDefinitionsToV3(
        SwaggerDocumentSecurityDefinitions v2Defs)
    {
        var v3Defs = new OpenApiDocumentSecurityDefinitions();
        foreach (var kvp in v2Defs)
        {
            var v2Scheme = kvp.Value;
            // Map common fields directly. V2 OAuth fields (Flow, AuthorizationUrl,
            // TokenUrl, Scopes) are structurally different in V3 and are not mapped
            // here; they fall through to JTokenProperties.
            v3Defs[kvp.Key] = new V3.Document.OpenApiDocumentSecurityScheme
            {
                Type = v2Scheme.Type,
                Description = v2Scheme.Description,
                Name = v2Scheme.Name,
                In = v2Scheme.In,
                JTokenProperties = v2Scheme.JTokenProperties
            };
        }
        return v3Defs;
    }

    private static V3.Document.OpenApiDocumentSecurityRequirement ConvertV2SecurityRequirementToV3(
        SwaggerDocumentSecurityRequirement v2Req)
    {
        var v3Req = new V3.Document.OpenApiDocumentSecurityRequirement();
        foreach (var kvp in v2Req)
        {
            v3Req[kvp.Key] = kvp.Value;
        }
        return v3Req;
    }

    private static async Task<string> ReadAllTextAsync(string filePath)
    {
        using var stream = new StreamReader(filePath, Encoding.UTF8);
        return await stream.ReadToEndAsync();
    }
}

