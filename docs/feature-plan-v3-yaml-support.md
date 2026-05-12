# Feature Plan: OpenAPI V3 + YAML Support

## Summary

Extend swagger-merge to support merging OpenAPI V3 (3.x) specification files in addition to Swagger V2, and support both JSON and YAML file formats for input and output.

## Current State

- The tool merges **Swagger V2 JSON files only**
- The `SwaggerDocument` model maps 1:1 to V2 fields (`host`, `basePath`, `schemes`, `definitions`, `securityDefinitions`)
- Definition resolution hardcodes `#/definitions/` ref paths
- `ISwaggerDocumentHandler` is JSON-only (`LoadFromJson`, `LoadFromFilePathAsync` reads JSON)
- Output config (`SwaggerOutputConfiguration`) has V2-only fields: `Host`, `BasePath`, `Schemes`, `SecurityDefinitions`
- All serialization uses `System.Text.Json` with source-generated contexts for AOT compatibility

## V2 vs V3 Structural Differences

| Swagger V2 (current) | OpenAPI V3 (needed) |
|---|---|
| `swagger: "2.0"` | `openapi: "3.x.x"` |
| `host` + `basePath` + `schemes` | `servers` array |
| `definitions` | `components.schemas` |
| `securityDefinitions` | `components.securitySchemes` |
| `parameters` (top-level) | `components.parameters` |
| `responses` (top-level) | `components.responses` |
| `produces` / `consumes` (doc + operation) | Per-operation `requestBody` + `content` media types |
| `$ref: "#/definitions/Foo"` | `$ref: "#/components/schemas/Foo"` |

## Design Approach

### Strategy: Separate V2 and V3 Document Models with a Shared Merge Abstraction

Rather than a unified document model (which would be fragile and hard to maintain), use separate strongly-typed models for V2 and V3, with a common interface for the merge handler to work against.

### Key Design Decisions

1. **Separate document models** - `SwaggerDocument` (V2, existing) and `OpenApiDocument` (V3, new). Each has its own serialization context. This keeps the models clean and avoids conditional logic.

2. **Spec version detection** - On load, peek at the document to determine version (`swagger: "2.0"` vs `openapi: "3.x.x"`) and deserialize to the correct model.

3. **Format detection** - Detect JSON vs YAML by file extension (`.json` vs `.yaml`/`.yml`) and content sniffing (leading `{` for JSON).

4. **YAML library** - Add `YamlDotNet` for YAML parsing. Convert YAML to an intermediate representation, then serialize to/from the typed models. Validate AOT compatibility.

5. **Merge handler per version** - `SwaggerMergeHandler` (V2, existing) and `OpenApiMergeHandler` (V3, new), both implementing a common interface. The correct handler is selected based on the detected spec version of the inputs.

6. **No cross-version merging** - V2 inputs merge to V2 output; V3 inputs merge to V3 output. Mixing V2 and V3 inputs is an error. (Cross-version conversion could be a future feature.)

7. **Configuration evolution** - Extend the configuration model to support both V2 and V3 output options, with the output config shape determined by the spec version.

---

## Work Breakdown

### Phase 1: Foundation - Format Detection + YAML Support

Add the ability to load and save documents in both JSON and YAML formats, with automatic format detection.

#### WI-1: Add YAML serialization support

**Scope**: SDK project

- Add `YamlDotNet` NuGet package to `SwaggerMerge.SDK.csproj`
- Validate AOT compatibility of YamlDotNet (check if it works with NativeAOT publish)
- If YamlDotNet is not AOT-compatible, evaluate alternatives or document the limitation

#### WI-2: Extend document handler for format detection

**Scope**: SDK project

- Rename/evolve `ISwaggerDocumentHandler` to support both formats:
  - `LoadFromFilePathAsync(string filePath)` - detect format from extension
  - `LoadFromContent(string content, DocumentFormat format)` - explicit format
  - `SaveToPathAsync(SwaggerDocument document, string filePath, DocumentFormat format)` - save in specified format
- Add `DocumentFormat` enum: `Json`, `Yaml`
- Implement format detection: `.json` = JSON, `.yaml`/`.yml` = YAML, content sniff as fallback
- Implement YAML deserialization to `SwaggerDocument` (V2 model, reusing existing types)
- Implement YAML serialization from `SwaggerDocument`
- Update `SwaggerDocumentHandler` implementation

#### WI-3: Add YAML test documents and tests

**Scope**: Test project

- Create YAML versions of existing test Swagger documents (`pet.swagger.yaml`, etc.)
- Add tests for YAML load, save, and round-trip
- Add tests for format detection logic
- Add tests for JSON-to-YAML and YAML-to-JSON conversion round-trips

#### WI-4: Update CLI for format-aware I/O

**Scope**: CLI project

- Update `SwaggerMergeConfigurationFileHandler` to detect input file formats
- Support YAML config files in addition to JSON config files (optional, stretch goal)
- Update output handling to respect the output file extension for format selection
- Add `--format` CLI option or auto-detect from output file extension

---

### Phase 2: OpenAPI V3 Document Model

Build the V3 document model and serialization, parallel to the existing V2 model.

#### WI-5: Create OpenAPI V3 document model

**Scope**: SDK project, new `Document/V3/` directory

- Create `OpenApiDocument` class with V3 fields:
  - `openapi` (version string, e.g., "3.0.3", "3.1.0")
  - `info` (reuse or create V3-compatible info model)
  - `servers` (list of `OpenApiServer` with `url`, `description`, `variables`)
  - `paths` (similar structure to V2 but with V3 operation model)
  - `components` (`OpenApiComponents` with `schemas`, `responses`, `parameters`, `examples`, `requestBodies`, `headers`, `securitySchemes`, `links`, `callbacks`)
  - `security` (list of security requirements)
  - `tags` (list of tag objects)
  - `externalDocs`
- Create V3-specific sub-models:
  - `OpenApiOperation` - includes `requestBody`, `callbacks`, `servers`
  - `OpenApiRequestBody` - `content` map of media types
  - `OpenApiMediaType` - `schema`, `examples`, `encoding`
  - `OpenApiServer` - `url`, `description`, `variables`
  - `OpenApiComponents` - container for all reusable objects
- Use `[JsonExtensionData]` for forward compatibility (same pattern as V2)

#### WI-6: Add V3 JSON serialization context

**Scope**: SDK project

- Create `OpenApiDocumentJsonSerializerContext` with source-generated contexts for all V3 types
- Create `OpenApiDocumentJson` with shared serializer options
- Ensure AOT compatibility

#### WI-7: Add V3 YAML serialization

**Scope**: SDK project

- Implement YAML serialization/deserialization for V3 model types
- Reuse the YAML infrastructure from Phase 1

#### WI-8: Implement spec version detection

**Scope**: SDK project

- Create a `SpecVersionDetector` that peeks at document content to determine:
  - V2: presence of `"swagger"` key with value `"2.0"`
  - V3: presence of `"openapi"` key with value starting with `"3."`
- Integrate into the document handler to auto-select the correct model for deserialization
- Return a `SpecVersion` enum: `SwaggerV2`, `OpenApiV3`

#### WI-9: Add V3 test documents and serialization tests

**Scope**: Test project

- Create V3 versions of test API documents (pet, store, todo, user) in both JSON and YAML
- Add tests for V3 document load, save, round-trip in both formats
- Add tests for spec version detection

---

### Phase 3: V3 Merge Logic

Implement the merge handler for OpenAPI V3 documents.

#### WI-10: Create V3 merge handler

**Scope**: SDK project

- Create `OpenApiMergeHandler` (or extend as partial classes, matching V2 pattern):
  - `OpenApiMergeHandler.cs` - main merge orchestrator
  - `OpenApiMergeHandler.Inputs.cs` - path transforms, title appending (same concepts as V2)
  - `OpenApiMergeHandler.Components.cs` - component schema resolution (replaces `Definitions.cs` pattern)
- Merge logic for V3-specific fields:
  - `paths` - same concept as V2, with path transforms and operation exclusions
  - `components.schemas` - union of all input schemas (equivalent of V2 `definitions`)
  - `components.parameters` - merge reusable parameters
  - `components.responses` - merge reusable responses
  - `components.securitySchemes` - merge security schemes
- `$ref` resolution uses `#/components/schemas/` instead of `#/definitions/`
- Unused component pruning (equivalent of V2 definition pruning)

#### WI-11: Create V3 merge configuration

**Scope**: SDK project

- Create `OpenApiOutputConfiguration` with V3 fields:
  - `Servers` (list of server objects, replaces `Host`/`BasePath`/`Schemes`)
  - `Components.SecuritySchemes` (replaces `SecurityDefinitions`)
  - `Security`
  - `Info` (title, version)
- Create `OpenApiMergeConfiguration` or extend existing to support V3 output
- Alternatively, use a polymorphic config model that supports both V2 and V3

#### WI-12: Add V3 merge tests

**Scope**: Test project

- Port all existing V2 merge tests to V3 equivalents:
  - Multi-document path merging
  - Title appending
  - Path transforms (StripStart, Prepend)
  - Component schema merging (equivalent of definition merging)
  - Server assignment
  - Operation exclusions with component pruning
- Add V3-specific tests:
  - Components merging (parameters, responses, security schemes)
  - Server array handling

---

### Phase 4: Unified Handler Selection + CLI Integration

Wire everything together so the tool auto-selects the correct handler based on input spec version.

#### WI-13: Implement merge handler routing

**Scope**: SDK project

- Create a unified entry point / factory that:
  - Inspects the spec version of input documents
  - Validates all inputs are the same version (error if mixed)
  - Routes to `SwaggerMergeHandler` (V2) or `OpenApiMergeHandler` (V3)
- Update `ISwaggerMergeHandler` or create a new unified interface

#### WI-14: Update CLI for V3 support

**Scope**: CLI project

- Update config file model to support V3 output options (`servers` instead of `host`/`basePath`/`schemes`)
- Update `SwaggerMergeConfigurationFileHandler` to detect V3 config and convert appropriately
- Update DI registration to include V3 handler
- Update validation logic for V3-specific config requirements

#### WI-15: Update CLI config schema

**Scope**: CLI project + documentation

- Design the config file format that supports both V2 and V3:
  ```json
  {
    "inputs": [...],
    "output": {
      "info": { "title": "...", "version": "..." },
      // V2 options (used when inputs are V2):
      "host": "...",
      "basePath": "...",
      "schemes": ["https"],
      // V3 options (used when inputs are V3):
      "servers": [{ "url": "https://api.example.com" }]
    }
  }
  ```
- Validate that V2 and V3 output options are not mixed

#### WI-16: Add end-to-end integration tests

**Scope**: Test project

- V2 JSON merge (existing, verify still passes)
- V2 YAML merge (new)
- V3 JSON merge (new)
- V3 YAML merge (new)
- Mixed format inputs (V3 JSON + V3 YAML = valid)
- Mixed version inputs (V2 + V3 = error)
- Output format respects file extension

---

### Phase 5: AOT Compatibility + Documentation

#### WI-17: Update AOT compatibility tests

**Scope**: AOT test project

- Add V3 document types to AOT test app
- Add V3 merge smoke test
- Add YAML round-trip in AOT context
- Verify zero IL warnings with new types

#### WI-18: Update documentation

**Scope**: Root + project READMEs

- Update root `README.md` to document V3 and YAML support
- Update SDK `README.md` with V3 usage examples
- Update CLI `README.md` with V3 config examples
- Add migration guide for existing V2-only users (no breaking changes expected)
- Document config file format for both V2 and V3

---

## Risk Assessment

| Risk | Impact | Mitigation |
|---|---|---|
| YamlDotNet not AOT-compatible | YAML support may not work with NativeAOT | Test early in Phase 1; evaluate alternatives (e.g., SharpYaml) or accept YAML as non-AOT |
| V3 spec complexity | V3 has significantly more features than V2 (callbacks, links, webhooks in 3.1) | Start with core V3 features (servers, components, paths); defer advanced features |
| Breaking changes to public API | SDK consumers may need code changes | Design V3 support as additive; keep existing V2 interfaces intact |
| YAML serialization fidelity | YAML round-trip may lose formatting or comments | Use standard YAML libraries; document that comments are not preserved |
| Mixed version detection edge cases | Malformed documents may not be detectable | Fail fast with clear error messages; require explicit version in config as fallback |

## Implementation Order

The phases are designed to be incremental and independently testable:

1. **Phase 1** (YAML) can ship independently as a useful feature even without V3
2. **Phase 2** (V3 model) is foundational for Phase 3
3. **Phase 3** (V3 merge) depends on Phase 2
4. **Phase 4** (integration) ties everything together
5. **Phase 5** (AOT + docs) finalizes the release

Each phase can be a separate PR or set of PRs for manageable review.

## Out of Scope (Future Work)

- **V2-to-V3 conversion** - Automatically converting V2 documents to V3 before merging
- **OpenAPI 3.1 specific features** - Webhooks, JSON Schema alignment (3.1 changes)
- **YAML config files** - Config file itself in YAML format (stretch goal in Phase 1)
- **Schema validation** - Validating input documents against the OpenAPI specification schema
- **Non-JSON/YAML formats** - No plans for other formats
