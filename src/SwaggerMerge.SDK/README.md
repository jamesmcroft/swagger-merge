# Swagger Merge SDK

[![GitHub release](https://img.shields.io/github/release/jamesmcroft/swagger-merge.svg)](https://github.com/jamesmcroft/swagger-merge/releases)
[![Build status](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml)
[![.NET Tool](https://img.shields.io/nuget/v/SwaggerMerge?label=dotnet%20tool)](https://www.nuget.org/packages/SwaggerMerge/)
[![SDK](https://img.shields.io/nuget/v/SwaggerMerge.SDK?label=sdk)](https://www.nuget.org/packages/SwaggerMerge.SDK/)

The Swagger Merge SDK allows you to process the merging of multiple Swagger files into a single Swagger file. This is useful for bringing together the API layer of a distributed service architecture where you wish to expose the APIs via a single API gateway.

The SDK supports merging both **Swagger V2** and **OpenAPI V3** specification files in **JSON and YAML** formats.

This SDK is used by the Swagger Merge CLI tool which is available as [a dotnet tool for you to install and use](https://www.nuget.org/packages/SwaggerMerge/).

> **Upgrading from v0.4.x?** See the [migration guide](../../docs/migration-v0.4-to-v1.0.md) for details on breaking changes and new namespace structure.

## Getting started

### Get the SDK

You can install the SDK into your dotnet application by running the following command:

```bash
dotnet add package SwaggerMerge.SDK
```

Or by adding the `SwaggerMerge.SDK` package in your NuGet package manager of choice.

### Merging Swagger V2 documents

The `SwaggerMergeHandler` is used to merge multiple Swagger V2 files into a single document using a `SwaggerMergeConfiguration` object.

```csharp
using SwaggerMerge.V2;
using SwaggerMerge.V2.Configuration;
using SwaggerMerge.V2.Document;
using SwaggerMerge.Common.Configuration.Input;

// Setup your configuration.
var config = new SwaggerMergeConfiguration();

SwaggerMergeHandler handler = new SwaggerMergeHandler();
SwaggerDocument merged = handler.Merge(config);
```

The configuration object is made up of several options that allow you to customize and tailor the inputs and output.

- `Inputs` - **Required**. An array of `SwaggerInputConfiguration` objects. Each input has the following properties:
  - `File` - **Required**. The `SwaggerDocument` input file.
  - `Path` - **Optional**. A configuration object for the paths of APIs with the following properties:
    - `Prepend` - **Optional**. A string to prepend to the path of each operation in the input file.
    - `StripStart` - **Optional**. A string to strip from the start of the path of each operation in the input file.
  - `Info` - **Optional**. An `InputInfoConfiguration` object for the info section with the following properties:
    - `Append` - **Optional**. A boolean value that determines whether the input file's info title should be appended to the output file's info title.
    - `Title` - **Optional**. A string to use as the title of the output file that is different to the original.
- `Output` - **Required**. A configuration object for the output file with the following properties:
  - `Info` - **Optional**. A configuration object for the info section with the following properties:
    - `Title` - **Optional**. A string to use as the title of the output file.
    - `Version` - **Optional**. A string to use as the version of the output file.
  - `BasePath` - **Optional**. A string to use as the base path of the output file.
  - `Host` - **Optional**. A string to use as the host of the output file.
  - `Schemes` - **Optional**. An array of strings to use as the schemes of the output file.
  - `SecurityDefinitions` - **Optional**. A configuration object for the security definitions of the output file with the following properties:
    - `Type` - **Optional**. A string to use as the type of the security definition.
    - `In` - **Optional**. A string to use as the in of the security definition.
    - `Name` - **Optional**. A string to use as the name of the security definition.
  - `Security` - **Optional**. An array of security requirements to use in the output file.

**Note**, `SwaggerMergeHandler` has an `ISwaggerMergeHandler` interface to ease the extensibility, testability, and support for dependency injection in your applications.

### Merging OpenAPI V3 documents

The `OpenApiMergeHandler` is used to merge multiple OpenAPI V3 files into a single document using an `OpenApiMergeConfiguration` object.

```csharp
using SwaggerMerge.V3;
using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Document;

// Setup your configuration.
var config = new OpenApiMergeConfiguration();

OpenApiMergeHandler handler = new OpenApiMergeHandler();
OpenApiDocument merged = handler.Merge(config);
```

The configuration follows a similar structure to the V2 configuration, adapted for the OpenAPI V3 specification.

### Handling `SwaggerDocument` objects

The SDK provides a `SwaggerDocumentHandler` that can be used to load `SwaggerDocument` objects from file paths, JSON strings, or YAML strings, and save them to an output file path.

The handler auto-detects the format (JSON or YAML) based on the file extension when loading from a file path.

```csharp
using SwaggerMerge.V2.Document;

SwaggerDocumentHandler handler = new SwaggerDocumentHandler();

// Load from a JSON file
SwaggerDocument fromJson = await handler.LoadFromFilePathAsync("path/to/file.json");

// Load from a YAML file (auto-detected by extension)
SwaggerDocument fromYaml = await handler.LoadFromFilePathAsync("path/to/file.yaml");

// Load from JSON content in memory
SwaggerDocument fromJsonString = handler.LoadFromJson(jsonContent);

// Load from YAML content in memory
SwaggerDocument fromYamlString = handler.LoadFromYaml(yamlContent);

// Save to a file path (format determined by extension)
await handler.SaveToFilePathAsync(fromJson, "path/to/output.json");
```

The same pattern applies to `OpenApiDocumentHandler` in `SwaggerMerge.V3.Document` for OpenAPI V3 documents.

**Note**, `SwaggerDocumentHandler` has an `ISwaggerDocumentHandler` interface to ease the extensibility, testability, and support for dependency injection in your applications.

### Example usage

```csharp
namespace Merger;

using SwaggerMerge.V2;
using SwaggerMerge.V2.Document;
using SwaggerMerge.V2.Configuration;

internal class SwaggerMerger
{
    private readonly ISwaggerMergeHandler mergeHandler;
    private readonly ISwaggerDocumentHandler documentHandler;

    public SwaggerMerger(
        ISwaggerMergeHandler mergeHandler,
        ISwaggerDocumentHandler documentHandler)
    {
        this.mergeHandler = mergeHandler;
        this.documentHandler = documentHandler;
    }

    public async Task MergeAsync(SwaggerMergeConfiguration config, string outputFilePath)
    {
        var output = this.mergeHandler.Merge(config);
        await this.documentHandler.SaveToPathAsync(output, outputFilePath);
    }
}
```

## Contributing 🤝🏻

Contributions, issues and feature requests are welcome!

Feel free to check the [issues page](https://github.com/jamesmcroft/swagger-merge/issues). You can also take a look at the [contributing guide](https://github.com/jamesmcroft/swagger-merge/blob/main/CONTRIBUTING.md).

We actively encourage you to jump in and help with any issues, and if you find one, don't forget to log it!

## Support this project 💗

As many developers know, projects like this are built and maintained in maintainers' spare time. If you find this project useful, please **Star** the repo.

## Author

👤 **James Croft**

- Website: <https://www.jamescroft.co.uk>
- Github: [@jamesmcroft](https://github.com/jamesmcroft)
- LinkedIn: [@jmcroft](https://linkedin.com/in/jmcroft)

## License

This project is made available under the terms and conditions of the [MIT license](LICENSE).
