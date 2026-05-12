# Migration Guide: v0.4.x to v1.0.0

This guide covers the breaking changes introduced in v1.0.0 and how to update your code.

## Summary of Changes

v1.0.0 restructures the SDK namespaces to cleanly separate Swagger V2 and OpenAPI V3 support, renames shared configuration types, and adds full OpenAPI V3 merge capability.

## Namespace Changes

All public types have moved to new namespaces. Update your `using` statements as follows:

### Merge Handlers

| v0.4.x | v1.0.0 |
|---|---|
| `SwaggerMerge.ISwaggerMergeHandler` | `SwaggerMerge.V2.ISwaggerMergeHandler` |
| `SwaggerMerge.SwaggerMergeHandler` | `SwaggerMerge.V2.SwaggerMergeHandler` |

### Document Types

| v0.4.x | v1.0.0 |
|---|---|
| `SwaggerMerge.Document.SwaggerDocument` | `SwaggerMerge.V2.Document.SwaggerDocument` |
| `SwaggerMerge.Document.SwaggerDocumentHandler` | `SwaggerMerge.V2.Document.SwaggerDocumentHandler` |
| `SwaggerMerge.Document.ISwaggerDocumentHandler` | `SwaggerMerge.V2.Document.ISwaggerDocumentHandler` |
| `SwaggerMerge.Document.IDocumentFormatHandler` | `SwaggerMerge.V2.Document.IDocumentFormatHandler` |
| `SwaggerMerge.Document.JsonDocumentFormatHandler` | `SwaggerMerge.V2.Document.JsonDocumentFormatHandler` |
| `SwaggerMerge.Document.YamlDocumentFormatHandler` | `SwaggerMerge.V2.Document.YamlDocumentFormatHandler` |
| `SwaggerMerge.Document.SwaggerDocumentJsonSerializerContext` | `SwaggerMerge.V2.Document.SwaggerDocumentJsonSerializerContext` |
| `SwaggerMerge.Document.DocumentFormat` | `SwaggerMerge.Common.Document.DocumentFormat` |
| `SwaggerMerge.Document.SpecVersion` | `SwaggerMerge.Common.Document.SpecVersion` |

### Configuration Types

| v0.4.x | v1.0.0 |
|---|---|
| `SwaggerMerge.Configuration.SwaggerMergeConfiguration` | `SwaggerMerge.V2.Configuration.SwaggerMergeConfiguration` |
| `SwaggerMerge.Configuration.Input.SwaggerInputConfiguration` | `SwaggerMerge.V2.Configuration.Input.SwaggerInputConfiguration` |
| `SwaggerMerge.Configuration.Output.SwaggerOutputConfiguration` | `SwaggerMerge.V2.Configuration.Output.SwaggerOutputConfiguration` |
| `SwaggerMerge.Configuration.Output.SwaggerOutputInfoConfiguration` | `SwaggerMerge.V2.Configuration.Output.SwaggerOutputInfoConfiguration` |

### Renamed Types

The following shared configuration types were renamed to remove the `Swagger` prefix, as they are used by both V2 and V3:

| v0.4.x | v1.0.0 |
|---|---|
| `SwaggerMerge.Configuration.Input.SwaggerInputPathConfiguration` | `SwaggerMerge.Common.Configuration.Input.InputPathConfiguration` |
| `SwaggerMerge.Configuration.Input.SwaggerInputInfoConfiguration` | `SwaggerMerge.Common.Configuration.Input.InputInfoConfiguration` |
| `SwaggerMerge.Configuration.Input.SwaggerInputPathOperationExclusionConfiguration` | `SwaggerMerge.Common.Configuration.Input.InputPathOperationExclusionConfiguration` |

### Exceptions

| v0.4.x | v1.0.0 |
|---|---|
| `SwaggerMerge.Exceptions.SwaggerMergeException` | `SwaggerMerge.Common.Exceptions.SwaggerMergeException` |

## Migration Steps

### 1. Update NuGet Package

```
dotnet add package SwaggerMerge.SDK --version 1.0.0
```

### 2. Find and Replace Namespaces

The simplest approach is to find and replace `using` statements across your project:

```
// Old
using SwaggerMerge;
using SwaggerMerge.Configuration;
using SwaggerMerge.Configuration.Input;
using SwaggerMerge.Configuration.Output;
using SwaggerMerge.Document;
using SwaggerMerge.Exceptions;

// New (for V2 usage)
using SwaggerMerge.V2;
using SwaggerMerge.V2.Configuration;
using SwaggerMerge.V2.Configuration.Input;
using SwaggerMerge.V2.Configuration.Output;
using SwaggerMerge.V2.Document;
using SwaggerMerge.Common.Configuration.Input;
using SwaggerMerge.Common.Document;
using SwaggerMerge.Common.Exceptions;
```

### 3. Rename Shared Config Types

If you reference path or info configuration types directly, update the type names:

```csharp
// Old
var pathConfig = new SwaggerInputPathConfiguration { StripStart = "/api" };
var infoConfig = new SwaggerInputInfoConfiguration { Append = true };

// New
var pathConfig = new InputPathConfiguration { StripStart = "/api" };
var infoConfig = new InputInfoConfiguration { Append = true };
```

### 4. No Behavioral Changes

The V2 merge logic is unchanged. All existing merge configurations and behaviors work exactly as before with only the namespace and type name updates described above.

## New in v1.0.0

### OpenAPI V3 Merge Support

v1.0.0 adds full support for merging OpenAPI 3.x documents. See the SDK README for usage details.

```csharp
using SwaggerMerge.V3;
using SwaggerMerge.V3.Configuration;
using SwaggerMerge.V3.Configuration.Input;
using SwaggerMerge.V3.Configuration.Output;
using SwaggerMerge.V3.Document;

var handler = new OpenApiMergeHandler();
var result = handler.Merge(new OpenApiMergeConfiguration
{
    Inputs = new[]
    {
        new OpenApiInputConfiguration { File = petDoc },
        new OpenApiInputConfiguration { File = storeDoc }
    },
    Output = new OpenApiOutputConfiguration
    {
        Info = new OpenApiOutputInfoConfiguration { Title = "Merged API", Version = "1.0" },
        Servers = new List<OpenApiServer>
        {
            new() { Url = "https://api.example.com" }
        }
    }
});
```

### YAML Support

Both V2 and V3 documents can be loaded and saved in JSON or YAML format. Format is auto-detected from file extension (`.json`, `.yaml`, `.yml`).

### CLI V3 Support

The CLI tool now auto-detects whether your input files are Swagger V2 or OpenAPI V3 and routes to the appropriate merge handler. All inputs must be the same spec version. For V3 config files, use `servers` instead of `host`/`basePath`/`schemes` in the output section. See the [CLI README](../src/SwaggerMerge/README.md) for configuration examples.
