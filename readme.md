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

Setup steps TBD.

## Hosting

Hosting for these tools will be via a Docker container with environment
variables; a kubernetes chart will be provided.

## Hosted git support

Standard support for `git fetch` via SSH will be provided. In addition, webhook
support for GitHub and AzureDevOps is planned.

[git-tools]: https://github.com/PrincipleStudios/scalable-git-branching-tools/