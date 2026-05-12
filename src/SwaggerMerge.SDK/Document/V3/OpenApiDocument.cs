namespace SwaggerMerge.Document.V3;

using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Extensions;

/// <summary>
/// Defines the detail of an OpenAPI 3.x document.
/// </summary>
public class OpenApiDocument
{
    /// <summary>
    /// Gets or sets the OpenAPI specification version.
    /// </summary>
    [JsonPropertyName("openapi"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string OpenApiVersion { get; set; } = "3.0.3";

    /// <summary>
    /// Gets or sets the metadata about the API.
    /// </summary>
    [JsonPropertyName("info"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiDocumentInfo Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the server objects providing connectivity information to a target server.
    /// </summary>
    [JsonPropertyName("servers"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OpenApiServer>? Servers { get; set; }

    /// <summary>
    /// Gets or sets the available paths and operations for the API.
    /// </summary>
    [JsonPropertyName("paths"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiDocumentPaths? Paths { get; set; } = new();

    /// <summary>
    /// Gets or sets the reusable components of the OpenAPI document.
    /// </summary>
    [JsonPropertyName("components"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiComponents? Components { get; set; }

    /// <summary>
    /// Gets or sets the security mechanisms that can be used across the API.
    /// </summary>
    [JsonPropertyName("security"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OpenApiDocumentSecurityRequirement>? Security { get; set; }

    /// <summary>
    /// Gets or sets a list of tags used by the specification with additional metadata.
    /// </summary>
    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OpenApiTag>? Tags { get; set; }

    /// <summary>
    /// Gets or sets additional external documentation.
    /// </summary>
    [JsonPropertyName("externalDocs"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiExternalDocs? ExternalDocs { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined OpenAPI properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines the metadata about the API.
/// </summary>
public class OpenApiDocumentInfo
{
    /// <summary>
    /// Gets or sets the title of the API.
    /// </summary>
    [JsonPropertyName("title"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the version of the API document.
    /// </summary>
    [JsonPropertyName("version"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets a short description of the API.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines a server object providing connectivity information to a target server.
/// </summary>
public class OpenApiServer
{
    /// <summary>
    /// Gets or sets the URL to the target host.
    /// </summary>
    [JsonPropertyName("url"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets an optional string describing the host designated by the URL.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines the relative paths to the individual endpoints.
/// </summary>
public class OpenApiDocumentPaths : Dictionary<string, OpenApiDocumentPathItem>;

/// <summary>
/// Defines the operations available on a single path.
/// </summary>
public class OpenApiDocumentPathItem : Dictionary<string, OpenApiDocumentOperation>;

/// <summary>
/// Defines the detail of a single API operation on a path.
/// </summary>
public class OpenApiDocumentOperation
{
    /// <summary>
    /// Gets or sets a list of tags for API documentation control.
    /// </summary>
    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Tags { get; set; }

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
    /// Gets or sets a list of parameters that are applicable for this operation.
    /// </summary>
    [JsonPropertyName("parameters"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OpenApiDocumentProperty>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the request body applicable for this operation.
    /// </summary>
    [JsonPropertyName("requestBody"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiRequestBody? RequestBody { get; set; }

    /// <summary>
    /// Gets or sets the list of possible responses as they are returned from executing this operation.
    /// </summary>
    [JsonPropertyName("responses"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, OpenApiDocumentProperty>? Responses { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this operation is deprecated.
    /// </summary>
    [JsonPropertyName("deprecated"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Deprecated { get; set; }

    /// <summary>
    /// Gets or sets the security mechanisms that can be used for this operation.
    /// </summary>
    [JsonPropertyName("security"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OpenApiDocumentSecurityRequirement>? Security { get; set; }

    /// <summary>
    /// Gets or sets an alternative server array to service this operation.
    /// </summary>
    [JsonPropertyName("servers"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OpenApiServer>? Servers { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined OpenAPI properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, OpenApiDocumentProperty>? AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined OpenAPI properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }

    private static OpenApiDocumentProperty ToOpenApiDocumentProperty(JsonElement? jsonObject)
    {
        if (jsonObject == null)
        {
            return new OpenApiDocumentProperty();
        }

        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        jsonObject.Value.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, OpenApiDocumentPropertyJsonSerializerContext.Default.OpenApiDocumentProperty) ??
               new OpenApiDocumentProperty();
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
            x => ToOpenApiDocumentProperty(x.Value));

        JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JsonSerializer.SerializeToElement(x.Value, OpenApiDocumentPropertyJsonSerializerContext.Default.OpenApiDocumentProperty));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties ??= new();
        JTokenProperties.AddRange(additionalProperties);
    }
}

/// <summary>
/// Defines the request body applicable for an operation.
/// </summary>
public class OpenApiRequestBody
{
    /// <summary>
    /// Gets or sets a brief description of the request body.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the content of the request body, keyed by media type.
    /// </summary>
    [JsonPropertyName("content"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, OpenApiMediaType>? Content { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request body is required.
    /// </summary>
    [JsonPropertyName("required"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines a media type object describing request/response content.
/// </summary>
public class OpenApiMediaType
{
    /// <summary>
    /// Gets or sets the schema defining the type used for the request body.
    /// </summary>
    [JsonPropertyName("schema"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiDocumentProperty? Schema { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines the reusable components of the OpenAPI document.
/// </summary>
public class OpenApiComponents
{
    /// <summary>
    /// Gets or sets the reusable schema objects.
    /// </summary>
    [JsonPropertyName("schemas"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, OpenApiDocumentProperty>? Schemas { get; set; }

    /// <summary>
    /// Gets or sets the reusable response objects.
    /// </summary>
    [JsonPropertyName("responses"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, OpenApiDocumentProperty>? Responses { get; set; }

    /// <summary>
    /// Gets or sets the reusable parameter objects.
    /// </summary>
    [JsonPropertyName("parameters"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, OpenApiDocumentProperty>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the reusable security scheme objects.
    /// </summary>
    [JsonPropertyName("securitySchemes"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiDocumentSecurityDefinitions? SecuritySchemes { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines the required security schemes to execute an operation.
/// </summary>
public class OpenApiDocumentSecurityRequirement : Dictionary<string, List<string>>;

/// <summary>
/// Defines the details of an object to hold security schemes.
/// </summary>
public class OpenApiDocumentSecurityDefinitions : Dictionary<string, OpenApiDocumentSecurityScheme>;

/// <summary>
/// Defines the security scheme that can be used by the operations.
/// </summary>
public class OpenApiDocumentSecurityScheme
{
    /// <summary>
    /// Gets or sets the type of security scheme.
    /// </summary>
    [JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets a short description for the security scheme.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the name of the header, query, or cookie parameter to be used.
    /// </summary>
    [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the API key.
    /// </summary>
    [JsonPropertyName("in"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? In { get; set; }

    /// <summary>
    /// Gets or sets the name of the HTTP Authorization scheme.
    /// </summary>
    [JsonPropertyName("scheme"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Scheme { get; set; }

    /// <summary>
    /// Gets or sets a hint to the client to identify how the bearer token is formatted.
    /// </summary>
    [JsonPropertyName("bearerFormat"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? BearerFormat { get; set; }

    /// <summary>
    /// Gets or sets the OpenId Connect URL to discover OAuth2 configuration values.
    /// </summary>
    [JsonPropertyName("openIdConnectUrl"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? OpenIdConnectUrl { get; set; }

    /// <summary>
    /// Gets or sets the OAuth flows configuration.
    /// </summary>
    [JsonPropertyName("flows"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiOAuthFlows? Flows { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }
}

/// <summary>
/// Defines the configuration for the OAuth flows supported by the security scheme.
/// </summary>
public class OpenApiOAuthFlows
{
    /// <summary>
    /// Gets or sets the configuration for the OAuth Implicit flow.
    /// </summary>
    [JsonPropertyName("implicit"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiOAuthFlow? Implicit { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the OAuth Resource Owner Password flow.
    /// </summary>
    [JsonPropertyName("password"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiOAuthFlow? Password { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the OAuth Client Credentials flow.
    /// </summary>
    [JsonPropertyName("clientCredentials"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiOAuthFlow? ClientCredentials { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the OAuth Authorization Code flow.
    /// </summary>
    [JsonPropertyName("authorizationCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiOAuthFlow? AuthorizationCode { get; set; }
}

/// <summary>
/// Defines the configuration details for a supported OAuth flow.
/// </summary>
public class OpenApiOAuthFlow
{
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
    /// Gets or sets the URL to be used for obtaining refresh tokens.
    /// </summary>
    [JsonPropertyName("refreshUrl"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RefreshUrl { get; set; }

    /// <summary>
    /// Gets or sets the available scopes for the OAuth2 security scheme.
    /// </summary>
    [JsonPropertyName("scopes"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, string>? Scopes { get; set; }
}

/// <summary>
/// Defines a tag object with additional metadata.
/// </summary>
public class OpenApiTag
{
    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a short description for the tag.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets additional external documentation for this tag.
    /// </summary>
    [JsonPropertyName("externalDocs"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiExternalDocs? ExternalDocs { get; set; }
}

/// <summary>
/// Defines an external documentation object.
/// </summary>
public class OpenApiExternalDocs
{
    /// <summary>
    /// Gets or sets a short description of the target documentation.
    /// </summary>
    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the URL for the target documentation.
    /// </summary>
    [JsonPropertyName("url"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Url { get; set; }
}

/// <summary>
/// Defines the detail of a generic OpenAPI document property.
/// </summary>
public class OpenApiDocumentProperty
{
    /// <summary>
    /// Gets or sets the reference string to a component.
    /// </summary>
    [JsonPropertyName("$ref"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets a definition of the parameter structure.
    /// </summary>
    [JsonPropertyName("schema"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiDocumentProperty? Schema { get; set; }

    /// <summary>
    /// Gets or sets the type of items in the array if the type is <b>array</b>.
    /// </summary>
    [JsonPropertyName("items"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OpenApiDocumentProperty? Items { get; set; }

    /// <summary>
    /// Gets or sets the properties of an item.
    /// </summary>
    [JsonPropertyName("properties"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, OpenApiDocumentProperty>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, OpenApiDocumentProperty>? AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the additional JSON properties that are not covered by the defined properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? JTokenProperties { get; set; }

    private static OpenApiDocumentProperty ToOpenApiDocumentProperty(JsonElement? jsonObject)
    {
        if (jsonObject == null)
        {
            return new OpenApiDocumentProperty();
        }

        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        jsonObject.Value.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, OpenApiDocumentPropertyJsonSerializerContext.Default.OpenApiDocumentProperty) ??
               new OpenApiDocumentProperty();
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
            x => ToOpenApiDocumentProperty(x.Value));

        JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JsonSerializer.SerializeToElement(x.Value, OpenApiDocumentPropertyJsonSerializerContext.Default.OpenApiDocumentProperty));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties ??= new();
        JTokenProperties.AddRange(additionalProperties);
    }
}
