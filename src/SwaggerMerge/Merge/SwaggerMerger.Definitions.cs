namespace SwaggerMerge.Merge;

using MADE.Collections;
using SwaggerMerge.Swagger;

internal static partial class SwaggerMerger
{
    private static SwaggerDocumentDefinitions GetUsedDefinitions(SwaggerDocument document)
    {
        void ProcessPropertyItemReference(
            KeyValuePair<string, SwaggerDocumentSchema> property,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (property.Value.Items?.Reference == null)
            {
                return;
            }

            var expectedDefinition = GetDefinitionByReference(property.Value.Items.Reference, document.Definitions);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || usedDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessDefinitionProperties(expectedDefinition, usedDefinitions);
        }

        void ProcessPropertyReference(
            KeyValuePair<string, SwaggerDocumentSchema> property,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (property.Value.Reference == null)
            {
                return;
            }

            var expectedDefinition = GetDefinitionByReference(property.Value.Reference, document.Definitions);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || usedDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessDefinitionProperties(expectedDefinition, usedDefinitions);
        }

        void ProcessDefinitionProperties(
            KeyValuePair<string, SwaggerDocumentSchema> definition,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (definition.Value.Properties == null)
            {
                return;
            }

            foreach (var property in definition.Value.Properties)
            {
                ProcessPropertyReference(property, usedDefinitions);
                ProcessPropertyItemReference(property, usedDefinitions);
            }
        }

        void ProcessResponseSchemaItemReference(
            SwaggerDocumentResponse response,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (response.Schema is not { Items.Reference: { } })
            {
                return;
            }

            var expectedDefinition =
                GetDefinitionByReference(response.Schema?.Reference, document.Definitions);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || usedDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessDefinitionProperties(expectedDefinition, usedDefinitions);
        }

        void ProcessResponseSchemaReference(
            SwaggerDocumentResponse response,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (response.Schema is not { Reference: { } })
            {
                return;
            }

            var expectedDefinition =
                GetDefinitionByReference(response.Schema?.Reference, document.Definitions);

            if (string.IsNullOrWhiteSpace(expectedDefinition.Key) || usedDefinitions.ContainsKey(expectedDefinition.Key))
            {
                return;
            }

            usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
            ProcessDefinitionProperties(expectedDefinition, usedDefinitions);
        }

        void ProcessParameterSchemaItemReference(
            SwaggerDocumentOperation operation,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var referenceParameter in operation.Parameters.Where(x =>
                         x.Schema is { Items.Reference: { } }))
            {
                var expectedDefinition =
                    GetDefinitionByReference(referenceParameter.Schema?.Items?.Reference, document.Definitions);

                if (string.IsNullOrWhiteSpace(expectedDefinition.Key) ||
                    usedDefinitions.ContainsKey(expectedDefinition.Key))
                {
                    continue;
                }

                usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
                ProcessDefinitionProperties(expectedDefinition, usedDefinitions);
            }
        }

        void ProcessParameterSchemaReference(
            SwaggerDocumentOperation operation,
            SwaggerDocumentDefinitions usedDefinitions)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (SwaggerDocumentParameter referenceParameter in operation.Parameters.Where(x =>
                         x.Schema is { Reference: { } }))
            {
                var expectedDefinition =
                    GetDefinitionByReference(referenceParameter.Schema?.Reference, document.Definitions);

                if (string.IsNullOrWhiteSpace(expectedDefinition.Key) ||
                    usedDefinitions.ContainsKey(expectedDefinition.Key))
                {
                    continue;
                }

                usedDefinitions.AddOrUpdate(expectedDefinition.Key, expectedDefinition.Value);
                ProcessDefinitionProperties(expectedDefinition, usedDefinitions);
            }
        }

        SwaggerDocumentDefinitions usedDefinitions = new SwaggerDocumentDefinitions();

        if (document.Definitions == null || document.Paths == null)
        {
            return usedDefinitions;
        }

        foreach (var operation in document.Paths.SelectMany(path => path.Value.Select(method => method.Value)))
        {
            if (operation.Parameters != null)
            {
                ProcessParameterSchemaReference(operation, usedDefinitions);
                ProcessParameterSchemaItemReference(operation, usedDefinitions);
            }

            if (operation.Responses != null)
            {
                foreach (var response in operation.Responses.Select(operationResponse => operationResponse.Value))
                {
                    ProcessResponseSchemaReference(response, usedDefinitions);
                    ProcessResponseSchemaItemReference(response, usedDefinitions);
                }
            }
        }

        return usedDefinitions;
    }

    private static KeyValuePair<string, SwaggerDocumentSchema> GetDefinitionByReference(
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