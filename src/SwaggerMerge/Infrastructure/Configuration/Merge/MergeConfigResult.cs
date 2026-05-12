namespace SwaggerMerge.Infrastructure.Configuration.Merge;

using SwaggerMerge.Common.Document;
using SwaggerMerge.V2.Configuration;
using SwaggerMerge.V3.Configuration;

/// <summary>
/// Carries the result of converting a config file, including the detected spec version
/// and the appropriate SDK configuration for V2 or V3 merging.
/// </summary>
internal sealed class MergeConfigResult
{
    /// <summary>
    /// Gets the detected specification version of the input documents.
    /// </summary>
    public required SpecVersion Version { get; init; }

    /// <summary>
    /// Gets the V2 merge configuration. Non-null when <see cref="Version"/> is <see cref="SpecVersion.SwaggerV2"/>.
    /// </summary>
    public SwaggerMergeConfiguration? V2Config { get; init; }

    /// <summary>
    /// Gets the V3 merge configuration. Non-null when <see cref="Version"/> is <see cref="SpecVersion.OpenApiV3"/>.
    /// </summary>
    public OpenApiMergeConfiguration? V3Config { get; init; }
}
