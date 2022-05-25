namespace SwaggerMerge.Swagger;

using Newtonsoft.Json;

/// <summary>
/// Defines the detail of a Swagger document.
/// </summary>
public class SwaggerDocument
{
    /// <summary>
    /// Gets or sets the Swagger specification version.
    /// </summary>
    [JsonProperty("swagger")]
    public string SwaggerVersion { get; set; } = "2.0";

    /// <summary>
    /// Gets or sets the metadata about the API.
    /// </summary>
    public SwaggerDocumentInfo Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the host (name or IP) serving the API.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the base path on which the API is served, which is relative to the host.
    /// </summary>
    public string? BasePath { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets a list of MIME types the APIs can produce.
    /// </summary>
    public List<string>? Produces { get; set; }

    /// <summary>
    /// Gets or sets the available paths and operations for the API.
    /// </summary>
    public SwaggerDocumentPaths? Paths { get; set; } = new();

    /// <summary>
    /// Gets or sets the data types produced and consumed by operations.
    /// </summary>
    public SwaggerDocumentDefinitions? Definitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the parameters to be reused across operations.
    /// </summary>
    public SwaggerDocumentParametersDefinitions? Parameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the responses to be reused across operations.
    /// </summary>
    public SwaggerDocumentResponsesDefinitions? Responses { get; set; } = new();

    /// <summary>
    /// Gets or sets the security scheme to be reused across the specification.
    /// </summary>
    public SwaggerDocumentSecurityDefinitions? SecurityDefinitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the security options available in the Swagger document.
    /// </summary>
    public List<SwaggerDocumentSecurityRequirement>? Security { get; set; } = new();

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
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
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets a short description of the security scheme.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the name of the header or query parameter to be used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the API key.
    /// </summary>
    public string? In { get; set; }

    /// <summary>
    /// Gets or sets the flow used by the OAuth2 security scheme.
    /// </summary>
    public string? Flow { get; set; }

    /// <summary>
    /// Gets or sets the authorization URL to be used for this flow.
    /// </summary>
    public string? AuthorizationUrl { get; set; }

    /// <summary>
    /// Gets or sets the token URL to be used for this flow.
    /// </summary>
    public string? TokenUrl { get; set; }

    /// <summary>
    /// Gets or sets the available scopes for the OAuth2 security scheme.
    /// </summary>
    public SwaggerDocumentScopes? Scopes { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Defines the list of available scopes for an OAuth2 security scheme.
/// </summary>
public class SwaggerDocumentScopes : Dictionary<string, object>
{
}

/// <summary>
/// Defines the details of an object to hold responses to be reused across operations.
/// </summary>
public class SwaggerDocumentResponsesDefinitions : Dictionary<string, SwaggerDocumentResponse>
{
}

/// <summary>
/// Defines the details of an object to hold parameters to be reused across operations.
/// </summary>
public class SwaggerDocumentParametersDefinitions : Dictionary<string, SwaggerDocumentParameter>
{
}

/// <summary>
/// Defines the details of an object to hold data types that can be consumed and produced by operations.
/// </summary>
public class SwaggerDocumentDefinitions : Dictionary<string, SwaggerDocumentSchema>
{
}

/// <summary>
/// Defines the metadata about the API.
/// </summary>
public class SwaggerDocumentInfo
{
    /// <summary>
    /// Gets or sets the title of the API.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the version of the API.
    /// </summary>
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
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets a short summary of what the operation does.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets a verbose explanation of the operation behavior.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unique string used to identify the operation.
    /// </summary>
    public string? OperationId { get; set; }

    /// <summary>
    /// Gets or sets a list of MIME types the APIs can produce.
    /// </summary>
    public List<string>? Produces { get; set; }

    /// <summary>
    /// Gets or sets a list of parameters that are applicable for this operation.
    /// </summary>
    public List<SwaggerDocumentParameter>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the list of possible responses as they are returned from executing this operation.
    /// </summary>
    public SwaggerDocumentResponses? Responses { get; set; }

    /// <summary>
    /// Gets or sets the transfer protocol of the API.
    /// </summary>
    public List<string>? Schemes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this operation is deprecated.
    /// </summary>
    public bool Deprecated { get; set; } = false;

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Defines the details for the expected responses of an operation.
/// </summary>
public class SwaggerDocumentResponses : Dictionary<string, SwaggerDocumentResponse>
{
}

/// <summary>
/// Defines the detail of a response of a single operation.
/// </summary>
public class SwaggerDocumentResponse
{
    /// <summary>
    /// Gets or sets the reference string to a definition.
    /// </summary>
    [JsonProperty("$ref")] public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets a short description of the response.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a list of headers that are sent with the response.
    /// </summary>
    public SwaggerDocumentHeaders? Headers { get; set; }

    /// <summary>
    /// Gets or sets a definition of the response structure.
    /// </summary>
    public SwaggerDocumentSchema? Schema { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Defines the list of headers that can be sent as part of a response
/// </summary>
public class SwaggerDocumentHeaders : Dictionary<string, SwaggerDocumentHeader>
{
}

/// <summary>
/// Defines the input and output data types.
/// </summary>
public class SwaggerDocumentSchema
{
    /// <summary>
    /// Gets or sets the reference string to a definition.
    /// </summary>
    [JsonProperty("$ref")] public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets the type of the object.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the extending format for the <see cref="Type"/>.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the type of items in the array if the <see cref="Type"/> is <b>array</b>.
    /// </summary>
    public SwaggerDocumentSchema? Items { get; set; }

    /// <summary>
    /// Gets or sets the properties of an item.
    /// </summary>
    public Dictionary<string, SwaggerDocumentSchema>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

/// <summary>
/// Defines the details of a parameter of a single operation.
/// </summary>
public class SwaggerDocumentParameter
{
    /// <summary>
    /// Gets or sets the reference string to a definition.
    /// </summary>
    [JsonProperty("$ref")] public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the location of the parameter.
    /// </summary>
    public string? In { get; set; }

    /// <summary>
    /// Gets or sets the brief description of the parameter.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the parameter is mandatory.
    /// </summary>
    public bool Required { get; set; } = false;

    /// <summary>
    /// Gets or sets the type of the parameter.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the extending format for the <see cref="Type"/>.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the type of items in the array if the <see cref="Type"/> is <b>array</b>.
    /// </summary>
    public SwaggerDocumentSchema? Items { get; set; }

    /// <summary>
    /// Gets or sets the properties of an item.
    /// </summary>
    public Dictionary<string, SwaggerDocumentSchema>? Properties { get; set; }

    /// <summary>
    /// Gets or sets a definition of the parameter structure.
    /// </summary>
    public SwaggerDocumentSchema? Schema { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

public class SwaggerDocumentHeader
{
    /// <summary>
    /// Gets or sets a short description of the header.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the type of the object.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the extending format for the <see cref="Type"/>.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the type of items in the array if the <see cref="Type"/> is <b>array</b>.
    /// </summary>
    public SwaggerDocumentSchema? Items { get; set; }

    /// <summary>
    /// Gets or sets the properties of an item.
    /// </summary>
    public Dictionary<string, SwaggerDocumentSchema>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the additional properties that are not covered by the defined Swagger properties.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}
