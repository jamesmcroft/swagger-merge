namespace SwaggerMerge.Common.Document;

using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
/// Defines the supported specification versions.
/// </summary>
public enum SpecVersion
{
    /// <summary>
    /// Swagger 2.0 specification.
    /// </summary>
    SwaggerV2,

    /// <summary>
    /// OpenAPI 3.x specification.
    /// </summary>
    OpenApiV3
}

/// <summary>
/// Detects the specification version of a Swagger/OpenAPI document from its content.
/// </summary>
public static class SpecVersionDetector
{
    private static readonly Regex SwaggerV2Regex = new(
        @"""swagger""\s*:\s*""2\.",
        RegexOptions.Compiled);

    private static readonly Regex OpenApiV3Regex = new(
        @"""openapi""\s*:\s*""3\.",
        RegexOptions.Compiled);

    private static readonly Regex SwaggerV2YamlRegex = new(
        @"^swagger\s*:\s*['""]?2\.",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex OpenApiV3YamlRegex = new(
        @"^openapi\s*:\s*['""]?3\.",
        RegexOptions.Compiled | RegexOptions.Multiline);

    /// <summary>
    /// Detects the specification version from the document content.
    /// </summary>
    /// <param name="content">The raw content of the document (JSON or YAML).</param>
    /// <returns>The detected <see cref="SpecVersion"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specification version cannot be determined.</exception>
    public static SpecVersion DetectVersion(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("Cannot detect specification version from empty content.");
        }

        var trimmed = content.TrimStart();

        // Try JSON-style detection first (works for JSON content)
        if (trimmed.StartsWith('{'))
        {
            if (SwaggerV2Regex.IsMatch(content))
            {
                return SpecVersion.SwaggerV2;
            }

            if (OpenApiV3Regex.IsMatch(content))
            {
                return SpecVersion.OpenApiV3;
            }
        }

        // YAML-style detection
        if (SwaggerV2YamlRegex.IsMatch(content))
        {
            return SpecVersion.SwaggerV2;
        }

        if (OpenApiV3YamlRegex.IsMatch(content))
        {
            return SpecVersion.OpenApiV3;
        }

        // JSON regex might miss if there are leading spaces in JSON keys - also check with YAML patterns on JSON
        if (SwaggerV2Regex.IsMatch(content))
        {
            return SpecVersion.SwaggerV2;
        }

        if (OpenApiV3Regex.IsMatch(content))
        {
            return SpecVersion.OpenApiV3;
        }

        throw new InvalidOperationException(
            "Cannot detect the specification version. The document must contain either \"swagger\": \"2.x\" or \"openapi\": \"3.x\".");
    }
}
