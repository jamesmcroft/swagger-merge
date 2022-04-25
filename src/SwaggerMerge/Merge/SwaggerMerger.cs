using MADE.Collections;
using SwaggerMerge.Serialization;
using SwaggerMerge.Swagger;

namespace SwaggerMerge.Merge
{
    internal static class SwaggerMerger
    {
        public static async Task MergeAsync(SwaggerMergeConfiguration config)
        {
            var output = new SwaggerDocument();

            var outputTitle = config.Output.Info?.Title ?? "";

            foreach (var inputConfig in config.Inputs)
            {
                var input = await JsonFile.LoadFileAsync<SwaggerDocument>(inputConfig.File);

                if (inputConfig.Info is {Append: true})
                {
                    if (inputConfig.Info.Title != null &&
                        !string.IsNullOrWhiteSpace(inputConfig.Info.Title))
                    {
                        outputTitle += " " + inputConfig.Info.Title;
                    }
                    else
                    {
                        outputTitle += input.Info.Title;
                    }
                }

                foreach (var path in input.Paths)
                {
                    var outputPath = path.Key;

                    if (inputConfig.Path?.StripStart != null && !string.IsNullOrWhiteSpace(inputConfig.Path.StripStart))
                    {
                        outputPath = outputPath.Substring(inputConfig.Path.StripStart.Length);
                    }

                    if (inputConfig.Path?.Prepend != null && !string.IsNullOrWhiteSpace(inputConfig.Path.Prepend))
                    {
                        outputPath = inputConfig.Path.Prepend + outputPath;
                    }

                    output.Paths.AddOrUpdate(outputPath, path.Value);
                }

                foreach (var definition in input.Definitions.Where(definition =>
                             !output.Definitions.ContainsKey(definition.Key)))
                {
                    output.Definitions.AddOrUpdate(definition.Key, definition.Value);
                }
            }

            output.Info.Title = outputTitle;
            output.Info.Version = config.Output.Info?.Version ?? "1.0";

            await JsonFile.SaveFileAsync(config.Output.File, output);

            Console.WriteLine($"Merged {config.Inputs.Count()} files into '{config.Output.File}'");
        }
    }
}