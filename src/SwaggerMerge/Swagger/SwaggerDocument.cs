using Newtonsoft.Json;

namespace SwaggerMerge.Swagger;

public class SwaggerDocument
{
    [JsonProperty("swagger")] public string SwaggerVersion { get; set; } = "2.0";

    [JsonProperty("info")]
    public SwaggerDocumentInfo Info { get; set; } = new();

    [JsonProperty("paths")]
    public Dictionary<string, object> Paths { get; set; } = new();

    [JsonProperty("definitions")]
    public Dictionary<string, object> Definitions { get; set; } = new();

    [JsonProperty("securityDefinitions")]
    public Dictionary<string, object> SecurityDefinitions { get; set; } = new();

    [JsonProperty("security")]
    public List<object> Security { get; set; } = new();
}