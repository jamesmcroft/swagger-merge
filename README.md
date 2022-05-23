# Swagger v2 JSON merge CLI tool

[![GitHub release](https://img.shields.io/github/release/jamesmcroft/swagger-merge.svg)](https://github.com/jamesmcroft/swagger-merge/releases)
[![Build status](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/jamesmcroft/swagger-merge/actions/workflows/ci.yml)
[![Twitter Followers](https://img.shields.io/twitter/follow/jamesmcroft?label=follow%20%40jamesmcroft&style=flat)](https://twitter.com/jamesmcroft)

The Swagger v2 merge CLI tool allows you to process the merging of multiple Swagger v2 JSON files into a single Swagger v2 JSON file. This is useful for bringing together the API layer of a distributed service architecture where you wish to expose the APIs via a single API gateway.

## Getting started

### Install the tool

```bash
dotnet tool install -g SwaggerMerge
```

### Configure your Swagger document merge

To use the CLI tool, you will need access to all of your Swagger v2 JSON input files, and you will need to create a configuration JSON file that will be used by the CLI tool to determine how to merge the input files together.

Here's an example of the format for this configuration file.

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
              "stripStart": "/foo"
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
      }
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

### Run the merge tool

Once you have your configuration file, it is simply a case of running CLI and passing the file as the first argument to the tool. 

```bash
swagger-merge config.json
```

This will merge all of the input files into a single output file at the configured path. And that's it!

## Contributing 🤝🏻

Contributions, issues and feature requests are welcome!

Feel free to check the [issues page](https://github.com/jamesmcroft/swagger-merge/issues). You can also take a look at the [contributing guide](https://github.com/jamesmcroft/swagger-merge/blob/main/CONTRIBUTING.md).

We actively encourage you to jump in and help with any issues, and if you find one, don't forget to log it!

## Support this project 💗

As many developers know, projects like this are built and maintained in spare time. If you find this project useful, please **Star** the repo.

## Author

👤 **James Croft**

* Website: https://www.jamescroft.co.uk
* Twitter: [@jamesmcroft](https://twitter.com/jamesmcroft)
* Github: [@jamesmcroft](https://github.com/jamesmcroft)
* LinkedIn: [@jmcroft](https://linkedin.com/in/jmcroft)

## License

This project is made available under the terms and conditions of the [MIT license](LICENSE).
