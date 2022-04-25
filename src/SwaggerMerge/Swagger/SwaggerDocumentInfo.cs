using Newtonsoft.Json;

namespace SwaggerMerge.Swagger;

public class SwaggerDocumentInfo
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }
}