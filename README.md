# Swagger Merge

[![GitHub release](https://img.shields.io/github/release/jamesmcroft/swagger-merge.svg)](https://github.com/jamesmcroft/swagger-merge/releases)
[![Build status](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml)
[![.NET Tool](https://img.shields.io/nuget/v/SwaggerMerge?label=dotnet%20tool)](https://www.nuget.org/packages/SwaggerMerge/)
[![SDK](https://img.shields.io/nuget/v/SwaggerMerge.SDK?label=sdk)](https://www.nuget.org/packages/SwaggerMerge.SDK/)

The Swagger Merge SDK & CLI tool allows you to process the merging of multiple Swagger files into a single Swagger file. This is useful for bringing together the API layer of a distributed service architecture where you wish to expose the APIs via a single API gateway.

The CLI tool currently supports merging Swagger V2 specification JSON files. It is not yet capable of merging Swagger V3 specification JSON or YAML files.

## Getting started

### Install the tool

If you want to just use the tool to merge your Swagger files, you can install and use the Swagger Merge CLI tool.

```bash
dotnet tool install -g SwaggerMerge
```

**Or update an existing install**

```bash
dotnet tool update -g SwaggerMerge
```

To use the tool, [follow the Swagger Merge CLI tool instructions](https://github.com/jamesmcroft/swagger-merge/blob/main/src/SwaggerMerge/README.md).

### Get the SDK

If you want to build your own applications on the Swagger Merge SDK that backs the CLI tool, you can install the Swagger Merge SDK into your dotnet application.

```bash
dotnet add package SwaggerMerge.SDK
```

Or by adding the `SwaggerMerge.SDK` package in your NuGet package manager of choice.

To use the SDK, [follow the Swagger Merge SDK instructions](https://github.com/jamesmcroft/swagger-merge/blob/main/src/SwaggerMerge.SDK/README.md).

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
