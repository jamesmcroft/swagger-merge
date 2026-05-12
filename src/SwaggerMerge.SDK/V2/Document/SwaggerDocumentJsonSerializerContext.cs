namespace SwaggerMerge.V2.Document;

using System.Text.Json.Serialization;

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocument"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocument))]
public partial class SwaggerDocumentJsonSerializerContext : JsonSerializerContext
{
}

/// <summary>
/// Defines the JSON context for serializing and deserializing <see cref="SwaggerDocumentProperty"/> objects.
/// </summary>
[JsonSerializable(typeof(SwaggerDocumentProperty))]
public partial class SwaggerDocumentPropertyJsonSerializerContext : JsonSerializerContext
{
}
