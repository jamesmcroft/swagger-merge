namespace SwaggerMerge.Merge;

using MADE.Collections;
using SwaggerMerge.Swagger;

internal static partial class SwaggerMerger
{
    private static SwaggerDocumentDefinitions GetUsedDefinitions(SwaggerDocument document)
    {
        if (document.Definitions == null || document.Paths == null)
        {
            return new SwaggerDocumentDefinitions();
        }

        var pathDefinitions = GetDocumentPathDefinitions(document);
        PopulateDefinitionDefinitions(pathDefinitions, new SwaggerDocumentDefinitions(pathDefinitions), document);

        return pathDefinitions;
    }

    private static void PopulateDefinitionDefinitions(
        SwaggerDocumentDefinitions pathDefinitions,
        SwaggerDocumentDefinitions definitions,
        SwaggerDocument document)
    {
        var definitionDefinitions = new SwaggerDocumentDefinitions();

        if (document.Definitions == null)
        {
            return;
        }

        foreach (var definition in definitions)
        {
            var definitionReferences = new List<string>();

            PopulateReferences(definitionReferences, definition.Value);

            var keyedReferences = definitionReferences.Select(r => r.Replace("#/definitions/", string.Empty));

            if (keyedReferences.All(pathDefinitions.ContainsKey))
            {
                continue;
            }

            PopulateReferenceDefinitions(definitionReferences, document.Definitions, definitionDefinitions);

            // Adds any new definitions to the path definitions.
            foreach (var d in definitionDefinitions.Where(d => !pathDefinitions.ContainsKey(d.Key)))
            {
                pathDefinitions.Add(d.Key, d.Value);
            }

            // If we do have them, lets have a look for that definitions definitions
            if (definitionDefinitions.Any())
            {
                PopulateDefinitionDefinitions(pathDefinitions, definitionDefinitions, document);
            }
        }

    }

    private static SwaggerDocumentDefinitions GetDocumentPathDefinitions(SwaggerDocument document)
    {
        SwaggerDocumentDefinitions definitions = new SwaggerDocumentDefinitions();

        var definedPathReferences = new List<string>();

        if (document.Paths == null || document.Definitions == null)
        {
            return definitions;
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

            if (operation.Responses != null && operation.Responses.Any())
            {
                foreach (var response in operation.Responses)
                {
                    PopulateReferences(definedPathReferences, response.Value);
                }
            }

            if (operation.AdditionalProperties == null || !operation.AdditionalProperties.Any())
            {
                continue;
            }

            foreach (var additionalProperty in operation.AdditionalProperties)
            {
                PopulateReferences(definedPathReferences, additionalProperty.Value);
            }
        }

        PopulateReferenceDefinitions(definedPathReferences, document.Definitions, definitions);

        return definitions;
    }

    private static void PopulateReferenceDefinitions(
        IEnumerable<string> references,
        SwaggerDocumentDefinitions allDefinitions,
        SwaggerDocumentDefinitions usedDefinitions)
    {
        foreach (var expectedDefinition in references
                     .Select(reference => GetDefinitionByReference(reference, allDefinitions))
                     .Where(expectedDefinition => !string.IsNullOrWhiteSpace(expectedDefinition.Key) &&
                                                  !usedDefinitions.ContainsKey(expectedDefinition.Key)))
        {
            usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
        }
    }

    private static void PopulateReferences(ICollection<string> references, SwaggerDocumentProperty data)
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

        if (data.AdditionalProperties == null || !data.AdditionalProperties.Any())
        {
            return;
        }

        foreach (var additionalProperty in data.AdditionalProperties)
        {
            PopulateReferences(references, additionalProperty.Value);
        }
    }

    private static KeyValuePair<string, SwaggerDocumentProperty> GetDefinitionByReference(
        string? reference,
        SwaggerDocumentDefinitions allDefinitions)
    {
        return allDefinitions.FirstOrDefault(
            definition =>
                definition.Key.Equals(
                    reference?.Replace("#/definitions/", string.Empty),
                    StringComparison.CurrentCultureIgnoreCase));
    }
}