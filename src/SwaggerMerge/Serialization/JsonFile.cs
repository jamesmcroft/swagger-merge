namespace SwaggerMerge.Serialization;

using Newtonsoft.Json;

internal static class JsonFile
{
    internal static readonly JsonSerializerSettings Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static async Task<T> LoadFileAsync<T>(string filePath)
        where T : class
    {
        var content = await File.ReadAllTextAsync(filePath);
        var deserializedContent = JsonConvert.DeserializeObject<T>(content, Settings);
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
        var content = JsonConvert.SerializeObject(data, Settings);
        await File.WriteAllTextAsync(filePath, content);
    }
}