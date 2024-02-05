# Scaled Git App

A web application containing tools to assist with the Scaled Git Branching
Strategy. This works in conjunction with the [git tools][git-tools] made for
that strategy.

This application is intended to:
- Assist all team members on a project (including PMs, QA Specialists, and
  Engineers) for visualizing the current state of dependencies on a project.
- Highlight issues, such as out-of-date downstreams or missing upstreams.
- Provide guidance for [git tools][git-tools] commands in identified situations.

## Development

Development will be orchestrated via msbuild.

Prerequisites:
- [.NET 8.0.x SDK][dotnet-8]
- [.NET 6.0.x Runtime][codegen-dotnet-version] (for the [Principle Studios
  OpenAPI code generators][ps-openapi-codegen])

To run locally, use one of the following options:

- Using Visual Studio:
    1. Open `./PrincipleStudios.ScaledGitApp.sln`.
    2. Set up local configuration (TODO)
    3. Debug or run the `Server` project.

- Using the `dotnet` CLI:
    1. Set up local configuration (TODO)
    2. Run the following commands in your terminal:
        ```sh
        cd Server
        dotnet run
        ```

## Hosting

Hosting for these tools will be via a Docker container with environment
variables; a kubernetes chart will be provided.

## Hosted git support

Standard support for `git fetch` via SSH will be provided. In addition, webhook
support for GitHub and AzureDevOps is planned.

[git-tools]: https://github.com/PrincipleStudios/scalable-git-branching-tools/
[dotnet-8]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
[codegen-dotnet-version]: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
[ps-openapi-codegen]: https://github.com/PrincipleStudios/principle-studios-openapi-generators
