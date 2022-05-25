namespace SwaggerMerge.Merge;

using Exceptions;
using SwaggerMerge.Merge.Configuration;

internal static partial class SwaggerMerger
{
    public static void ValidateConfiguration(SwaggerMergeConfiguration config)
    {
        if (!config.Inputs.Any())
        {
            throw new SwaggerMergeException("At least 1 input file must be specified");
        }

        if (config.Inputs.Any(input => string.IsNullOrWhiteSpace(input.File)))
        {
            throw new SwaggerMergeException("All input file paths must be specified");
        }

        if (string.IsNullOrWhiteSpace(config.Output.File))
        {
            throw new SwaggerMergeException("The output file path must be specified");
        }
    }
}