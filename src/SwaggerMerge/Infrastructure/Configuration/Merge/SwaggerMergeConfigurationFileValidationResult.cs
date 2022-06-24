namespace SwaggerMerge.Infrastructure.Configuration.Merge;

internal class SwaggerMergeConfigurationFileValidationResult
{
    public SwaggerMergeConfigurationFileValidationResult(bool isValid, IEnumerable<string>? errors = default)
    {
        this.IsValid = isValid;
        this.Errors = errors;
    }

    public bool IsValid { get; }

    public IEnumerable<string>? Errors { get; }
}