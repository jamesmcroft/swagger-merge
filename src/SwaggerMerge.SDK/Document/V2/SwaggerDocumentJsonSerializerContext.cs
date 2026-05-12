namespace SwaggerMerge.Document;

using System.Text.Json.Serialization;

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocument"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocument))]
public partial class SwaggerDocumentJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentSecurityRequirement"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentSecurityRequirement))]
public partial class SwaggerDocumentSecurityRequirementJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentSecurityDefinitions"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentSecurityDefinitions))]
public partial class SwaggerDocumentSecurityDefinitionsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentSecurityScheme"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentSecurityScheme))]
public partial class SwaggerDocumentSecuritySchemeJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentScopes"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentScopes))]
public partial class SwaggerDocumentScopesJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentDefinitions"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentDefinitions))]
public partial class SwaggerDocumentDefinitionsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentInfo"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentInfo))]
public partial class SwaggerDocumentInfoJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentPaths"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentPaths))]
public partial class SwaggerDocumentPathsJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentPathItem"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentPathItem))]
public partial class SwaggerDocumentPathItemJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentOperation"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentOperation))]
public partial class SwaggerDocumentOperationJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentProperty"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentProperty))]
public partial class SwaggerDocumentPropertyJsonSerializerContext : JsonSerializerContext
{
}
