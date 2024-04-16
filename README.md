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
- [.NET 7.0.x Runtime][codegen-dotnet-version] (for the [Principle Studios
  OpenAPI code generators][ps-openapi-codegen])
- [pnpm][pnpm-setup]

To run locally, use one of the following options:

- Using Visual Studio:
  1. Open `./PrincipleStudios.ScaledGitApp.sln`.
  2. Set up local configuration (see below)
  3. Debug or run the `Server` project.

- Using the `dotnet` CLI:
  1. Set up local configuration (see below)
  2. Run the following commands in your terminal:
     ```sh
     cd Server
     dotnet run
     ```

- Within the `ui` folder:
  1. Set up local configuration (see below)
  2. Run the following commands in your terminal:
     ```sh
     cd ui
     pnpm start
     ```

### Local Configuration (Optional)

The project may run locally without any local configuration with limited
functionality. To configure the application for local development:

1. Create a `.env` file in the root of the repository
2. Set up [desired environment variables][docs-env-variables]

### Other components

- **Jaeger**

    If you have `docker-compose` installed and your container runtime started at
    the time of building, Jaeger will be automatically started at
    http://localhost:16686/ for collecting local OpenTelementry data.

## Hosting

Hosting for these tools will be via a Docker container with environment
variables; a kubernetes chart will be provided.

## Hosted git support

Standard support for `git fetch` via SSH will be provided. In addition, webhook
support for GitHub and AzureDevOps is planned.

[git-tools]: https://github.com/PrincipleStudios/scalable-git-branching-tools/
[dotnet-8]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
[codegen-dotnet-version]: https://dotnet.microsoft.com/en-us/download/dotnet/7.0
[ps-openapi-codegen]: https://github.com/PrincipleStudios/principle-studios-openapi-generators
[pnpm-setup]: https://pnpm.io/installation
[docs-env-variables]: docs/env.md