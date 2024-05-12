namespace SwaggerMerge.Infrastructure.Configuration.Merge;

internal sealed class SwaggerMergeConfigurationFileValidationResult(bool isValid, IEnumerable<string>? errors = default)
{
    public bool IsValid { get; } = isValid;

    public IEnumerable<string>? Errors { get; } = errors;
}
