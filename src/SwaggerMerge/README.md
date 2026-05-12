# Swagger Merge CLI tool

[![GitHub release](https://img.shields.io/github/release/jamesmcroft/swagger-merge.svg)](https://github.com/jamesmcroft/swagger-merge/releases)
[![Build status](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml)
[![.NET Tool](https://img.shields.io/nuget/v/SwaggerMerge?label=dotnet%20tool)](https://www.nuget.org/packages/SwaggerMerge/)
[![SDK](https://img.shields.io/nuget/v/SwaggerMerge.SDK?label=sdk)](https://www.nuget.org/packages/SwaggerMerge.SDK/)

The Swagger Merge CLI tool allows you to process the merging of multiple Swagger files into a single Swagger file. This is useful for bringing together the API layer of a distributed service architecture where you wish to expose the APIs via a single API gateway.

The CLI tool supports merging both **Swagger V2** and **OpenAPI V3** specification files in **JSON and YAML** formats. The spec version is auto-detected from your input files, and the output format is determined by the output file extension.

This tool uses the Swagger Merge SDK which is available as [a NuGet package for you to use in your own applications for merging Swagger files](https://www.nuget.org/packages/SwaggerMerge.SDK/).

## Getting started

### Install the tool

```bash
dotnet tool install -g SwaggerMerge
```

**Or update an existing install**

```bash
dotnet tool update -g SwaggerMerge
```

### Configure your Swagger document merge

To use the CLI tool, you will need access to all of your input files (JSON or YAML), and you will need to create a configuration JSON file that will be used by the CLI tool to determine how to merge the input files together.

The tool auto-detects whether your inputs are Swagger V2 or OpenAPI V3 and routes to the appropriate merge handler. All inputs must be the same spec version.

#### Swagger V2 example

Here's an example configuration for merging Swagger V2 files.

```json
{
  "inputs": [
      {
          "file": "./projects.swagger.json"
      },
      {
          "file": "./users.swagger.json",
          "path": {
              "prepend": "/api/users",
              "stripStart": "/foo",
          },
          "info": {
              "append": true,
              "title": " Including Users",
          }
      }
  ],
  "output": {
      "file": "./api.swagger.json",
      "info": {
          "title": "Services",
          "version": "1.0"
      },
      "basePath": "/",
      "host": "localhost:8080",
      "schemes": [
          "https"
      ],
      "securityDefinitions": {
          "ApiKeyAuth": {
              "type": "apiKey",
              "in": "header",
              "name": "Authorization"
          }
      },
      "security": [
          {
              "ApiKeyAuth": []
          }
      ]  
  }
}
```

The configuration file is made up of several options that allow you to customize and tailor the inputs and output of the tool.

- `inputs` - **Required**. An array of input files. Each input file is a JSON object with the following properties:
  - `file` - **Required**. The path to the input file.
  - `path` - **Optional**. A configuration object for the paths of APIs with the following properties:
    - `prepend` - **Optional**. A string to prepend to the path of each operation in the input file.
    - `stripStart` - **Optional**. A string to strip from the start of the path of each operation in the input file.
  - `info` - **Optional**. A configuration object for the info section with the following properties:
    - `append` - **Optional**. A boolean value that determines whether the input file's info title should be appended to the output file's info title.
    - `title` - **Optional**. A string to use as the title of the output file that is different to the original.
- `output` - **Required**. A configuration object for the output file with the following properties:
  - `file` - **Required**. The path to the output file.
  - `info` - **Optional**. A configuration object for the info section with the following properties:
    - `title` - **Optional**. A string to use as the title of the output file.
    - `version` - **Optional**. A string to use as the version of the output file.
  - `basePath` - **Optional**. A string to use as the base path of the output file.
  - `host` - **Optional**. A string to use as the host of the output file.
  - `schemes` - **Optional**. An array of strings to use as the schemes of the output file.
  - `securityDefinitions` - **Optional**. A configuration object for the security definitions of the output file with the following properties:
    - `type` - **Optional**. A string to use as the type of the security definition.
    - `in` - **Optional**. A string to use as the in of the security definition.
    - `name` - **Optional**. A string to use as the name of the security definition.
  - `security` - **Optional**. An array of security requirements to use in the output file.

#### OpenAPI V3 example

For OpenAPI V3 inputs, use `servers` instead of `host`/`basePath`/`schemes`:

```json
{
  "inputs": [
      {
          "file": "./pets.openapi.json"
      },
      {
          "file": "./store.openapi.yaml",
          "path": {
              "prepend": "/api/store",
              "stripStart": "/v1"
          }
      }
  ],
  "output": {
      "file": "./api.openapi.yaml",
      "info": {
          "title": "Merged API",
          "version": "1.0"
      },
      "servers": [
          {
              "url": "https://api.example.com",
              "description": "Production"
          }
      ],
      "securityDefinitions": {
          "BearerAuth": {
              "type": "http",
              "scheme": "bearer",
              "bearerFormat": "JWT"
          }
      },
      "security": [
          {
              "BearerAuth": []
          }
      ]
  }
}
```

The V3 output configuration supports:

- `servers` - **Optional**. An array of server objects with `url` and `description` properties.
- `securityDefinitions` - **Optional**. Maps to `components.securitySchemes` in the merged output.
- `security` - **Optional**. An array of security requirements.
- `info` - **Optional**. Same as V2.

> **Note:** The `openapi` version in the merged output is automatically inferred from the input documents (the highest version among inputs is used).

### Run the merge tool

Once you have your configuration file, it is simply a case of running CLI and passing the file as the first argument to the tool.

```bash
swagger-merge -c config.json
```

This will merge all of the input files into a single output file at the configured path. And that's it!

## Contributing 🤝🏻

Contributions, issues and feature requests are welcome!

Feel free to check the [issues page](https://github.com/jamesmcroft/swagger-merge/issues). You can also take a look at the [contributing guide](https://github.com/jamesmcroft/swagger-merge/blob/main/CONTRIBUTING.md).

We actively encourage you to jump in and help with any issues, and if you find one, don't forget to log it!

## Support this project 💗

As many developers know, projects like this are built and maintained in maintainers' spare time. If you find this project useful, please **Star** the repo.

## Author

👤 **James Croft**

- Website: <https://www.jamescroft.co.uk>
- Github: [@jamesmcroft](https://github.com/jamesmcroft)
- LinkedIn: [@jmcroft](https://linkedin.com/in/jmcroft)

## License

This project is made available under the terms and conditions of the [MIT license](LICENSE).
