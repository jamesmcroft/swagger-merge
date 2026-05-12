namespace SwaggerMerge.V3.Document;

using System.Text.Json.Serialization;

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocument"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocument))]
public partial class OpenApiDocumentJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentInfo"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentInfo))]
public partial class OpenApiDocumentInfoJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiServer"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiServer))]
public partial class OpenApiServerJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentPaths"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentPaths))]
public partial class OpenApiDocumentPathsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentPathItem"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentPathItem))]
public partial class OpenApiDocumentPathItemJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentOperation"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentOperation))]
public partial class OpenApiDocumentOperationJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiRequestBody"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiRequestBody))]
public partial class OpenApiRequestBodyJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiMediaType"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiMediaType))]
public partial class OpenApiMediaTypeJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiComponents"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiComponents))]
public partial class OpenApiComponentsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentSecurityRequirement"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentSecurityRequirement))]
public partial class OpenApiDocumentSecurityRequirementJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentSecurityDefinitions"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentSecurityDefinitions))]
public partial class OpenApiDocumentSecurityDefinitionsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentSecurityScheme"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentSecurityScheme))]
public partial class OpenApiDocumentSecuritySchemeJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiOAuthFlows"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiOAuthFlows))]
public partial class OpenApiOAuthFlowsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiOAuthFlow"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiOAuthFlow))]
public partial class OpenApiOAuthFlowJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiTag"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiTag))]
public partial class OpenApiTagJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiExternalDocs"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiExternalDocs))]
public partial class OpenApiExternalDocsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentProperty"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentProperty))]
public partial class OpenApiDocumentPropertyJsonSerializerContext : JsonSerializerContext
{
}
