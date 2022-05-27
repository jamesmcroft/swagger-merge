namespace SwaggerMerge.Swagger;

using System.Globalization;
using System.Runtime.Serialization;
using MADE.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serialization;

/// <summary>
/// Defines the detail of a Swagger document.
/// </summary>
public class SwaggerDocument
{
    /// <summary>
    /// Gets or sets the Swagger specification version.
    /// </summary>
    [JsonProperty("swagger", NullValueHandling = NullValueHandling.Ignore)]
    public string SwaggerVersion { get; set; } = "2.0";

    /// <summary>
    /// Gets or sets the metadata about the API.
    /// </summary>
    [JsonProperty("info", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentInfo Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the host (name or IP) serving the API.
    /// </summary>
    [JsonProperty("host", NullValueHandling = NullValueHandling.Ignore)]
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the base path on which the API is served, which is relative to the host.
    /// </summary>
    [JsonProperty("basePath", NullValueHandling = NullValueHandling.Ignore)]
    public string? BasePath { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    [JsonProperty("schemes", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets a list of MIME types the APIs can produce.
    /// </summary>
    [JsonProperty("produces", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Produces { get; set; }

    /// <summary>
    /// Gets or sets the available paths and operations for the API.
    /// </summary>
    [JsonProperty("paths", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentPaths? Paths { get; set; } = new();

    /// <summary>
    /// Gets or sets the data types produced and consumed by operations.
    /// </summary>
    [JsonProperty("definitions", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentDefinitions? Definitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the parameters to be reused across operations.
    /// </summary>
    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, SwaggerDocumentProperty>? Parameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the responses to be reused across operations.
    /// </summary>
    [JsonProperty("responses", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, SwaggerDocumentProperty>? Responses { get; set; } = new();

    /// <summary>
    /// Gets or sets the security scheme to be reused across the specification.
    /// </summary>
    [JsonProperty("securityDefinitions", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentSecurityDefinitions? SecurityDefinitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the security options available in the Swagger document.
    /// </summary>
    [JsonProperty("security", NullValueHandling = NullValueHandling.Ignore)]
    public List<SwaggerDocumentSecurityRequirement>? Security { get; set; } = new();

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    [JsonExtensionData] public Dictionary<string, JToken>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JToken? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
        JsonTextWriter jw = new JsonTextWriter(sw);
        jsonObject.WriteTo(jw);
        var json = sw.ToString();
        return JsonConvert.DeserializeObject<SwaggerDocumentProperty>(json, JsonFile.Settings) ?? new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        var objectTokens = JTokenProperties?.Where(x => x.Value is JObject).ToList();

        if (objectTokens == null || !objectTokens.Any())
        {
            return;
        }

        this.AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value as JObject));

        this.JTokenProperties.RemoveRange(objectTokens);
    }


    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JToken.FromObject(x.Value));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties.AddRange(additionalProperties);
    }
}

/// <summary>
/// Defines the required security schemes to execute an operation.
/// </summary>
public class SwaggerDocumentSecurityRequirement : Dictionary<string, List<string>>
{
}

/// <summary>
/// Defines the details of an object to hold security schemes to be reused across the specification.
/// </summary>
public class SwaggerDocumentSecurityDefinitions : Dictionary<string, SwaggerDocumentSecurityScheme>
{
}

/// <summary>
/// Defines the security scheme that can be used by the operations.
/// </summary>
public class SwaggerDocumentSecurityScheme
{
    /// <summary>
    /// Gets or sets the type of security scheme.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets a short description of the security scheme.
    /// </summary>
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the name of the header or query parameter to be used.
    /// </summary>
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the API key.
    /// </summary>
    [JsonProperty("in", NullValueHandling = NullValueHandling.Ignore)]
    public string? In { get; set; }

    /// <summary>
    /// Gets or sets the flow used by the OAuth2 security scheme.
    /// </summary>
    [JsonProperty("flow", NullValueHandling = NullValueHandling.Ignore)]
    public string? Flow { get; set; }

    /// <summary>
    /// Gets or sets the authorization URL to be used for this flow.
    /// </summary>
    [JsonProperty("authorizationUrl", NullValueHandling = NullValueHandling.Ignore)]
    public string? AuthorizationUrl { get; set; }

    /// <summary>
    /// Gets or sets the token URL to be used for this flow.
    /// </summary>
    [JsonProperty("tokenUrl", NullValueHandling = NullValueHandling.Ignore)]
    public string? TokenUrl { get; set; }

    /// <summary>
    /// Gets or sets the available scopes for the OAuth2 security scheme.
    /// </summary>
    [JsonProperty("scopes", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentScopes? Scopes { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    [JsonExtensionData] public Dictionary<string, JToken>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JToken? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
        JsonTextWriter jw = new JsonTextWriter(sw);
        jsonObject.WriteTo(jw);
        var json = sw.ToString();
        return JsonConvert.DeserializeObject<SwaggerDocumentProperty>(json, JsonFile.Settings) ?? new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        var objectTokens = JTokenProperties?.Where(x => x.Value is JObject).ToList();

        if (objectTokens == null || !objectTokens.Any())
        {
            return;
        }

        this.AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value as JObject));

        this.JTokenProperties.RemoveRange(objectTokens);
    }


    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JToken.FromObject(x.Value));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties.AddRange(additionalProperties);
    }
}

/// <summary>
/// Defines the list of available scopes for an OAuth2 security scheme.
/// </summary>
public class SwaggerDocumentScopes : Dictionary<string, object>
{
}

/// <summary>
/// Defines the details of an object to hold data types that can be consumed and produced by operations.
/// </summary>
public class SwaggerDocumentDefinitions : Dictionary<string, SwaggerDocumentProperty>
{
    public SwaggerDocumentDefinitions()
    {
    }

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
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the version of the API.
    /// </summary>
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public string Version { get; set; }
}

/// <summary>
/// Defines the relative paths to the individual endpoints.
/// </summary>
public class SwaggerDocumentPaths : Dictionary<string, SwaggerDocumentPathItem>
{
}

/// <summary>
/// Defines the operations available on a single path.
/// </summary>
public class SwaggerDocumentPathItem : Dictionary<string, SwaggerDocumentOperation>
{
}

/// <summary>
/// Defines the detail of a single API operation on a path.
/// </summary>
public class SwaggerDocumentOperation
{
    /// <summary>
    /// Gets or sets a list of tags for API documentation control.
    /// </summary>
    [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets a short summary of what the operation does.
    /// </summary>
    [JsonProperty("summary", NullValueHandling = NullValueHandling.Ignore)]
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets a verbose explanation of the operation behavior.
    /// </summary>
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unique string used to identify the operation.
    /// </summary>
    [JsonProperty("operationId", NullValueHandling = NullValueHandling.Ignore)]
    public string? OperationId { get; set; }

    /// <summary>
    /// Gets or sets a list of MIME types the APIs can produce.
    /// </summary>
    [JsonProperty("produces", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Produces { get; set; }

    /// <summary>
    /// Gets or sets a list of parameters that are applicable for this operation.
    /// </summary>
    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public List<SwaggerDocumentProperty>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the list of possible responses as they are returned from executing this operation.
    /// </summary>
    [JsonProperty("responses", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, SwaggerDocumentProperty>? Responses { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    [JsonProperty("schemes", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this operation is deprecated.
    /// </summary>
    [JsonProperty("deprecated", NullValueHandling = NullValueHandling.Ignore)]
    public bool Deprecated { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    [JsonExtensionData] public Dictionary<string, JToken>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JToken? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
        JsonTextWriter jw = new JsonTextWriter(sw);
        jsonObject.WriteTo(jw);
        var json = sw.ToString();
        return JsonConvert.DeserializeObject<SwaggerDocumentProperty>(json, JsonFile.Settings) ?? new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        var objectTokens = JTokenProperties?.Where(x => x.Value is JObject).ToList();

        if (objectTokens == null || !objectTokens.Any())
        {
            return;
        }

        this.AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value as JObject));

        this.JTokenProperties.RemoveRange(objectTokens);
    }


    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JToken.FromObject(x.Value));

        if (additionalProperties == null)
        {
            return;
        }

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
    [JsonProperty("$ref", NullValueHandling = NullValueHandling.Ignore)]
    public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets a definition of the parameter structure.
    /// </summary>
    [JsonProperty("schema", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentProperty? Schema { get; set; }

    /// <summary>
    /// Gets or sets the type of items in the array if the type is <b>array</b>.
    /// </summary>
    [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
    public SwaggerDocumentProperty? Items { get; set; }

    /// <summary>
    /// Gets or sets the properties of an item.
    /// </summary>
    [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, SwaggerDocumentProperty>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, SwaggerDocumentProperty>? AdditionalProperties { get; set; }

    [JsonExtensionData] public Dictionary<string, JToken>? JTokenProperties { get; set; }

    private static SwaggerDocumentProperty ToSwaggerDocumentProperty(JToken? jsonObject)
    {
        if (jsonObject == null)
        {
            return new SwaggerDocumentProperty();
        }

        using StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
        JsonTextWriter jw = new JsonTextWriter(sw);
        jsonObject.WriteTo(jw);
        var json = sw.ToString();
        return JsonConvert.DeserializeObject<SwaggerDocumentProperty>(json, JsonFile.Settings) ?? new SwaggerDocumentProperty();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        var objectTokens = JTokenProperties?.Where(x => x.Value is JObject).ToList();

        if (objectTokens == null || !objectTokens.Any())
        {
            return;
        }

        this.AdditionalProperties = objectTokens.ToDictionary(
            x => x.Key,
            x => ToSwaggerDocumentProperty(x.Value as JObject));

        this.JTokenProperties.RemoveRange(objectTokens);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        var additionalProperties = AdditionalProperties?.ToDictionary(
            x => x.Key,
            x => JToken.FromObject(x.Value));

        if (additionalProperties == null)
        {
            return;
        }

        JTokenProperties.AddRange(additionalProperties);
    }
}