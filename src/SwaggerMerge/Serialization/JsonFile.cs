using Newtonsoft.Json;

namespace SwaggerMerge.Serialization
{
    internal static class JsonFile
    {
        public static async Task<T> LoadFileAsync<T>(string filePath) where T : class
        {
            var content = await File.ReadAllTextAsync(filePath);
            var deserializedContent = JsonConvert.DeserializeObject<T>(content);
            if (deserializedContent == null)
            {
                throw new InvalidOperationException(
                    $"File '{filePath}' could not be loaded correctly as it is not in the correct format.");
            }

            return deserializedContent;
        }

        public static async Task SaveFileAsync<T>(string filePath, T data) where T : class
        {
            var content = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, content);
        }
    }
}