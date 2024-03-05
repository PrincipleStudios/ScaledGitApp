# Environment Variables

Whether working locally or using the container image, most settings may be
configured via environment variables.

See [common scenarios](#common-scenarios)

## GIT__WORKINGDIRECTORY

The working directory for the project using the Scaled Git Branching model. If
the working directory is empty, the repository as specified by `GIT__REPOSITORY`
will be cloned.

## GIT__REPOSITORY

The URL from which the repository should be cloned. If the
`GIT__WORKINGDIRECTORY` is not empty, the repository will not be cloned.

## GIT__GITTOOLSDIRECTORY

The directory where the [Scalable Git Branching Tools][git-tools] are already
cloned.

# Common Scenarios

## Working locally

```
GIT__WORKINGDIRECTORY=C:\Users\youruser\source\my-project
GIT__GITTOOLSDIRECTORY=C:\Users\youruser\source\scalable-git-branching-tools
```

[git-tools]: https://github.com/PrincipleStudios/scalable-git-branching-tools/
