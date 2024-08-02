# Swagger Merge SDK

[![GitHub release](https://img.shields.io/github/release/jamesmcroft/swagger-merge.svg)](https://github.com/jamesmcroft/swagger-merge/releases)
[![Build status](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml)
[![.NET Tool](https://img.shields.io/nuget/v/SwaggerMerge?label=dotnet%20tool)](https://www.nuget.org/packages/SwaggerMerge/)
[![SDK](https://img.shields.io/nuget/v/SwaggerMerge.SDK?label=sdk)](https://www.nuget.org/packages/SwaggerMerge.SDK/)

The Swagger Merge SDK allows you to process the merging of multiple Swagger files into a single Swagger file. This is useful for bringing together the API layer of a distributed service architecture where you wish to expose the APIs via a single API gateway.

The SDK currently supports merging Swagger V2 specification JSON files. It is not yet capable of merging Swagger V3 specification JSON or YAML files.

This SDK is used by the Swagger Merge CLI tool which is available as [a dotnet tool for you to install and use](https://www.nuget.org/packages/SwaggerMerge/).

## Getting started

### Get the SDK

You can install the SDK into your dotnet application by running the following command:

```bash
dotnet add package SwaggerMerge.SDK
```

Or by adding the `SwaggerMerge.SDK` package in your NuGet package manager of choice.

### Using the `SwaggerMergeHandler`

The `SwaggerMergeHandler` is a simple class that can be used to merge multiple Swagger files into a single Swagger file using a `SwaggerMergeConfiguration` object.

```csharp
using SwaggerMerge;
using SwaggerMerge.Configuration;
using SwaggerMerge.Document;

// Setup your configuration.
var config = new SwaggerMergeConfiguration();

SwaggerMergeHandler handler = new SwaggerMergeHandler();
SwaggerDocument merged = handler.Merge(config);
```

The configuration object is made up of several options that allow you to customize and tailor the inputs and output.

- `Inputs` - **Required**. An array of input files. Each input file is a JSON object with the following properties:
  - `File` - **Required**. The `SwaggerDocument` input file.
  - `Path` - **Optional**. A configuration object for the paths of APIs with the following properties:
    - `Prepend` - **Optional**. A string to prepend to the path of each operation in the input file.
    - `StripStart` - **Optional**. A string to strip from the start of the path of each operation in the input file.
  - `Info` - **Optional**. A configuration object for the info section with the following properties:
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

### Handling `SwaggerDocument` objects

The SDK provides a `SwaggerDocumentHandler` that can be used to load `SwaggerDocument` objects from file paths or JSON strings, and save them to an output file path.

The purpose of the handler is to ensure that the Swagger document JSON is formatted correctly using the expected converter settings.

```csharp
using SwaggerMerge.Document;

SwaggerDocumentHandler handler = new SwaggerDocumentHandler();

// Load with file path to a JSON file
SwaggerDocument documentFromFile = await handler.LoadFromFilePathAsync("path/to/file.json");

// Load with JSON content in memory
string swaggerJsonContent; // Populate with your Swagger JSON content
SwaggerDocument documentFromJson = handler.LoadFromJson(swaggerJsonContent);

// Save as JSON to a file path
await handler.SaveToFilePathAsync(documentFromFile, "path/to/file.json");
```

**Note**, `SwaggerDocumentHandler` has an `ISwaggerDocumentHandler` interface to ease the extensibility, testability, and support for dependency injection in your applications.

### Example usage

```csharp
namespace Merger;

using SwaggerMerge;
using SwaggerMerge.Document;

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

    public async Task MergeAsync(SwaggerMergeConfiguration config)
    {
        var output = this.mergeHandler.Merge(config);
        await this.documentHandler.SaveToPathAsync(output, configFile.Output.File);
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
