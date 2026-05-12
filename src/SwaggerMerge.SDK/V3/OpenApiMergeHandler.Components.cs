namespace SwaggerMerge.V3;

using System.Text;
using System.Text.Json;
using SwaggerMerge.V3.Document;
using SwaggerMerge.Common.Extensions;

/// <summary>
/// Defines the handler logic for merging OpenAPI V3 document component schemas.
/// </summary>
public partial class OpenApiMergeHandler
{
    private static Dictionary<string, OpenApiDocumentProperty> GetUsedComponentSchemas(OpenApiDocument document)
    {
        if (document.Components?.Schemas == null || document.Paths == null)
        {
            return new Dictionary<string, OpenApiDocumentProperty>();
        }

        var pathSchemas = GetDocumentPathComponentSchemas(document);
        PopulateReferenceComponentSchemas(pathSchemas, new Dictionary<string, OpenApiDocumentProperty>(pathSchemas), document);

        return pathSchemas;
    }

    private static void PopulateReferenceComponentSchemas(
        Dictionary<string, OpenApiDocumentProperty> pathSchemas,
        Dictionary<string, OpenApiDocumentProperty> schemas,
        OpenApiDocument document)
    {
        var schemaReferences = new Dictionary<string, OpenApiDocumentProperty>();

        if (document.Components?.Schemas == null)
        {
            return;
        }

        foreach (var schema in schemas)
        {
            var references = new List<string>();

            PopulateReferences(references, schema.Value);

            var keyedReferences = references.Select(r => r.Replace("#/components/schemas/", string.Empty));

            if (keyedReferences.All(pathSchemas.ContainsKey))
            {
                continue;
            }

            PopulateReferenceComponentSchemasFromReferences(references, document.Components.Schemas, schemaReferences);

            foreach (var s in schemaReferences.Where(s => !pathSchemas.ContainsKey(s.Key)))
            {
                pathSchemas.Add(s.Key, s.Value);
            }

            if (schemaReferences.Any())
            {
                PopulateReferenceComponentSchemas(pathSchemas, schemaReferences, document);
            }
        }
    }

    private static Dictionary<string, OpenApiDocumentProperty> GetDocumentPathComponentSchemas(OpenApiDocument document)
    {
        var schemas = new Dictionary<string, OpenApiDocumentProperty>();

        var definedPathReferences = new List<string>();

        if (document.Paths == null || document.Components?.Schemas == null)
        {
            return schemas;
        }

        foreach (var operation in document.Paths.SelectMany(path => path.Value.Select(method => method.Value)))
        {
            if (operation.Parameters != null)
            {
                foreach (var parameter in operation.Parameters)
                {
                    PopulateReferences(definedPathReferences, parameter);
                }
            }

            if (operation.RequestBody?.Content != null)
            {
                foreach (var content in operation.RequestBody.Content)
                {
                    if (content.Value.Schema != null)
                    {
                        PopulateReferences(definedPathReferences, content.Value.Schema);
                    }
                }
            }

            if (operation.Responses != null && operation.Responses.Any())
            {
                foreach (var response in operation.Responses)
                {
                    PopulateReferences(definedPathReferences, response.Value);
                }
            }

            if (operation.JTokenProperties == null || !operation.JTokenProperties.Any())
            {
                continue;
            }

            foreach (var jTokenProperty in operation.JTokenProperties)
            {
                if (jTokenProperty.Value.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
                {
                    continue;
                }

                var property = JsonElementToOpenApiDocumentProperty(jTokenProperty.Value);
                PopulateReferences(definedPathReferences, property);
            }
        }

        PopulateReferenceComponentSchemasFromReferences(definedPathReferences, document.Components.Schemas, schemas);

        return schemas;
    }

    private static void PopulateReferenceComponentSchemasFromReferences(
        IEnumerable<string> references,
        Dictionary<string, OpenApiDocumentProperty> allSchemas,
        Dictionary<string, OpenApiDocumentProperty> usedSchemas)
    {
        foreach (var expectedSchema in references
                     .Select(reference => GetComponentSchemaByReference(reference, allSchemas))
                     .Where(expectedSchema => !string.IsNullOrWhiteSpace(expectedSchema.Key) &&
                                              !usedSchemas.ContainsKey(expectedSchema.Key)))
        {
            usedSchemas.AddOrUpdate(expectedSchema.Key, expectedSchema.Value);
        }
    }

    private static void PopulateReferences(ICollection<string> references, OpenApiDocumentProperty data)
    {
        if (data.Reference != null)
        {
            if (!references.Contains(data.Reference))
            {
                references.Add(data.Reference);
            }
        }

        if (data.Items != null)
        {
            PopulateReferences(references, data.Items);
        }

        if (data.Schema != null)
        {
            PopulateReferences(references, data.Schema);
        }

        if (data.Properties != null && data.Properties.Any())
        {
            foreach (var property in data.Properties)
            {
                PopulateReferences(references, property.Value);
            }
        }

        if (data.JTokenProperties == null || !data.JTokenProperties.Any())
        {
            return;
        }

        foreach (var jTokenProperty in data.JTokenProperties)
        {
            if (jTokenProperty.Value.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
            {
                continue;
            }

            var property = JsonElementToOpenApiDocumentProperty(jTokenProperty.Value);
            PopulateReferences(references, property);
        }
    }

    private static OpenApiDocumentProperty JsonElementToOpenApiDocumentProperty(JsonElement element)
    {
        using var sw = new MemoryStream();
        using var jw = new Utf8JsonWriter(sw);
        element.WriteTo(jw);
        jw.Flush();
        var json = Encoding.UTF8.GetString(sw.ToArray());
        return JsonSerializer.Deserialize(json, OpenApiDocumentPropertyJsonSerializerContext.Default.OpenApiDocumentProperty)
               ?? new OpenApiDocumentProperty();
    }

    private static KeyValuePair<string, OpenApiDocumentProperty> GetComponentSchemaByReference(
        string? reference,
        Dictionary<string, OpenApiDocumentProperty> allSchemas)
    {
        return allSchemas.FirstOrDefault(
            schema =>
                schema.Key.Equals(
                    reference?.Replace("#/components/schemas/", string.Empty),
                    StringComparison.CurrentCultureIgnoreCase));
    }
}
