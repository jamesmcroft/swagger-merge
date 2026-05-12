namespace SwaggerMerge.Document;

using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Extensions;

/// <summary>
/// Defines the detail of a Swagger document.
/// </summary>
public class SwaggerDocument
{
    /// <summary>
    /// Gets or sets the Swagger specification version.
    /// </summary>
    [JsonPropertyName("swagger"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string SwaggerVersion { get; set; } = "2.0";

    /// <summary>
    /// Gets or sets the metadata about the API.
    /// </summary>
    [JsonPropertyName("info"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentInfo Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the host (name or IP) serving the API.
    /// </summary>
    [JsonPropertyName("host"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the base path on which the API is served, which is relative to the host.
    /// </summary>
    [JsonPropertyName("basePath"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? BasePath { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    [JsonPropertyName("schemes"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets a list of MIME types the APIs can produce.
    /// </summary>
    [JsonPropertyName("produces"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Produces { get; set; }

    /// <summary>
    /// Gets or sets the available paths and operations for the API.
    /// </summary>
    [JsonPropertyName("paths"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentPaths? Paths { get; set; } = new();

    /// <summary>
    /// Gets or sets the data types produced and consumed by operations.
    /// </summary>
    [JsonPropertyName("definitions"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentDefinitions? Definitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the parameters to be reused across operations.
    /// </summary>
    [JsonPropertyName("parameters"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, SwaggerDocumentProperty>? Parameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the responses to be reused across operations.
    /// </summary>
    [JsonPropertyName("responses"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, SwaggerDocumentProperty>? Responses { get; set; } = new();

    /// <summary>
    /// Gets or sets the security scheme to be reused across the specification.
    /// </summary>
    [JsonPropertyName("securityDefinitions"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentSecurityDefinitions? SecurityDefinitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the security options available in the Swagger document.
    /// </summary>
    [JsonPropertyName("security"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<SwaggerDocumentSecurityRequirement>? Security { get; set; } = new();

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JsonElement? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        jsonObject.Value.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty) ??
               new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (JTokenProperties == null)
        {
            return;
        }

        var objectTokens = JTokenProperties.ToList();

        if (!objectTokens.Any())
        {
            return;
        }

        AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value));

        JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JsonSerializer.SerializeToElement(x.Value, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties ??= new();
        JTokenProperties.AddRange(additionalProperties);
    }
}

/// <summary>
/// Defines the required security schemes to execute an operation.
/// </summary>
public class SwaggerDocumentSecurityRequirement : Dictionary<string, List<string>>;

/// <summary>
/// Defines the details of an object to hold security schemes to be reused across the specification.
/// </summary>
public class SwaggerDocumentSecurityDefinitions : Dictionary<string, SwaggerDocumentSecurityScheme>;

/// <summary>
/// Defines the security scheme that can be used by the operations.
/// </summary>
public class SwaggerDocumentSecurityScheme
{
    /// <summary>
    /// Gets or sets the type of security scheme.
    /// </summary>
    [JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets a short description of the security scheme.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the name of the header or query parameter to be used.
    /// </summary>
    [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the API key.
    /// </summary>
    [JsonPropertyName("in"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? In { get; set; }

    /// <summary>
    /// Gets or sets the flow used by the OAuth2 security scheme.
    /// </summary>
    [JsonPropertyName("flow"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Flow { get; set; }

    /// <summary>
    /// Gets or sets the authorization URL to be used for this flow.
    /// </summary>
    [JsonPropertyName("authorizationUrl"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AuthorizationUrl { get; set; }

    /// <summary>
    /// Gets or sets the token URL to be used for this flow.
    /// </summary>
    [JsonPropertyName("tokenUrl"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TokenUrl { get; set; }

    /// <summary>
    /// Gets or sets the available scopes for the OAuth2 security scheme.
    /// </summary>
    [JsonPropertyName("scopes"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentScopes? Scopes { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JsonElement? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        jsonObject.Value.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty) ??
               new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (JTokenProperties == null)
        {
            return;
        }

        var objectTokens = JTokenProperties.ToList();

        if (!objectTokens.Any())
        {
            return;
        }

        AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value));

        JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JsonSerializer.SerializeToElement(x.Value, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties ??= new();
        JTokenProperties.AddRange(additionalProperties);
    }
}

/// <summary>
/// Defines the list of available scopes for an OAuth2 security scheme.
/// </summary>
public class SwaggerDocumentScopes : Dictionary<string, object>;

/// <summary>
/// Defines the details of an object to hold data types that can be consumed and produced by operations.
/// </summary>
public class SwaggerDocumentDefinitions : Dictionary<string, SwaggerDocumentProperty>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerDocumentDefinitions"/> class.
    /// </summary>
    public SwaggerDocumentDefinitions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerDocumentDefinitions"/> class with the specified document definitions.
    /// </summary>
    /// <param name="dictionary">The dictionary of document definitions.</param>
    public SwaggerDocumentDefinitions(IDictionary<string, SwaggerDocumentProperty> dictionary)
        : base(dictionary)
    {
    }
}

/// <summary>
/// Defines the metadata about the API.
/// </summary>
public class SwaggerDocumentInfo
{
    /// <summary>
    /// Gets or sets the title of the API.
    /// </summary>
    [JsonPropertyName("title"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the version of the API.
    /// </summary>
    [JsonPropertyName("version"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Version { get; set; }
}

/// <summary>
/// Defines the relative paths to the individual endpoints.
/// </summary>
public class SwaggerDocumentPaths : Dictionary<string, SwaggerDocumentPathItem>;

/// <summary>
/// Defines the operations available on a single path.
/// </summary>
public class SwaggerDocumentPathItem : Dictionary<string, SwaggerDocumentOperation>;

/// <summary>
/// Defines the detail of a single API operation on a path.
/// </summary>
public class SwaggerDocumentOperation
{
    /// <summary>
    /// Gets or sets a list of tags for API documentation control.
    /// </summary>
    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets a short summary of what the operation does.
    /// </summary>
    [JsonPropertyName("summary"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets a verbose explanation of the operation behavior.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unique string used to identify the operation.
    /// </summary>
    [JsonPropertyName("operationId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? OperationId { get; set; }

    /// <summary>
    /// Gets or sets a list of MIME types the APIs can produce.
    /// </summary>
    [JsonPropertyName("produces"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Produces { get; set; }

    /// <summary>
    /// Gets or sets a list of parameters that are applicable for this operation.
    /// </summary>
    [JsonPropertyName("parameters"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<SwaggerDocumentProperty>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the list of possible responses as they are returned from executing this operation.
    /// </summary>
    [JsonPropertyName("responses"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, SwaggerDocumentProperty>? Responses { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    [JsonPropertyName("schemes"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this operation is deprecated.
    /// </summary>
    [JsonPropertyName("deprecated"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Deprecated { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JsonElement? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        jsonObject.Value.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty) ??
               new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (JTokenProperties == null)
        {
            return;
        }

        var objectTokens = JTokenProperties.ToList();

        if (!objectTokens.Any())
        {
            return;
        }

        AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value));

        JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JsonSerializer.SerializeToElement(x.Value, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties ??= new();
        JTokenProperties.AddRange(additionalProperties);
    }
}

/// <summary>
/// Defines the detail of a generic Swagger document property.
/// </summary>
public class SwaggerDocumentProperty
{
    /// <summary>
    /// Gets or sets the reference string to a definition.
    /// </summary>
    [JsonPropertyName("$ref"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets a definition of the parameter structure.
    /// </summary>
    [JsonPropertyName("schema"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentProperty? Schema { get; set; }

    /// <summary>
    /// Gets or sets the type of items in the array if the type is <b>array</b>.
    /// </summary>
    [JsonPropertyName("items"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SwaggerDocumentProperty? Items { get; set; }

    /// <summary>
    /// Gets or sets the properties of an item.
    /// </summary>
    [JsonPropertyName("properties"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, SwaggerDocumentProperty>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JsonElement? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        jsonObject.Value.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty) ??
               new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (JTokenProperties == null)
        {
            return;
        }

        var objectTokens = JTokenProperties.ToList();

        if (!objectTokens.Any())
        {
            return;
        }

        AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value));

        JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JsonSerializer.SerializeToElement(x.Value, SwaggerDocumentPropertyJsonSerializerContext.Default.SwaggerDocumentProperty));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties ??= new();
        JTokenProperties.AddRange(additionalProperties);
    }
}
