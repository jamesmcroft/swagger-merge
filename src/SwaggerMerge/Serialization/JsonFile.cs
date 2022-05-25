namespace SwaggerMerge.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

internal static class JsonFile
{
    private static readonly JsonSerializerSettings InputSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        Formatting = Formatting.Indented
    };

    private static readonly JsonSerializerSettings OutputSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static async Task<T> LoadFileAsync<T>(string filePath)
        where T : class
    {
        var content = await File.ReadAllTextAsync(filePath);
        var deserializedContent = JsonConvert.DeserializeObject<T>(content, InputSettings);
        if (deserializedContent == null)
        {
            throw new InvalidOperationException(
                $"File '{filePath}' could not be loaded correctly as it is not in the correct format.");
        }

        return deserializedContent;
    }

    public static async Task SaveFileAsync<T>(string filePath, T data)
        where T : class
    {
        var content = JsonConvert.SerializeObject(data, OutputSettings);
        await File.WriteAllTextAsync(filePath, content);
    }
}