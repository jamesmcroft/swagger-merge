using SwaggerMerge.Merge;
using SwaggerMerge.Merge.Exceptions;
using SwaggerMerge.Serialization;

namespace SwaggerMerge;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            if (args == null || args.Length != 1)
            {
                throw new SwaggerMergeException("Please provide the Swagger merge configuration file.");
            }

            var config = await GetSwaggerMergeConfigurationAsync(args[0]);
            await SwaggerMerger.MergeAsync(config);
        }
        catch (SwaggerMergeException sme)
        {
            Console.WriteLine(sme.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while merging Swagger documents. Detail: {e}");
        }
    }

    private static async Task<SwaggerMergeConfiguration> GetSwaggerMergeConfigurationAsync(string configFilePath)
    {
        SwaggerMergeConfiguration? config;

        try
        {
            config = await JsonFile.LoadFileAsync<SwaggerMergeConfiguration>(configFilePath);
            Directory.SetCurrentDirectory(Path.GetDirectoryName(configFilePath));
        }
        catch (Exception)
        {
            throw new SwaggerMergeException("The provided Swagger merge configuration file is not valid.");
        }

        return config;
    }
}