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
/// Defines the JSON context for serializing and deserializing <see cref="OpenApiDocumentProperty"/> objects.
/// </summary>
[JsonSerializable(typeof(OpenApiDocumentProperty))]
public partial class OpenApiDocumentPropertyJsonSerializerContext : JsonSerializerContext
{
}
