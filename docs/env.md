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

## AUTH__AUTHENTICATION__GITHUB__CLIENTID

The Client ID for a GitHub Application used for authentication.

## AUTH__AUTHENTICATION__GITHUB__CLIENTSECRET

The Client Secret for a GitHub Application used for authentication.

## AUTH__AUTHENTICATION__MICROSOFT__CLIENTID

The Client ID for a Microsoft Azure AD Application used for authentication. See
[how to set up a web application][ms-auth].

## AUTH__AUTHENTICATION__MICROSOFT__CLIENTSECRET

The Client Secret for a Microsoft Azure AD Application used for authentication.
See [how to set up a web application][ms-auth].

## AUTH__AUTHENTICATION__MICROSOFT__TENANTID

The Tenant ID for a Microsoft Azure AD Application used for authentication when
not set up for a multi-organization app.

## AUTH__ALLOWEDUSERS__{n}

0-based list of usernames to allow. If provided, all other usernames will be
denied access to git-related APIs.

For example:

```
AUTH__ALLOWEDUSERS__0=mdekrey
AUTH__ALLOWEDUSERS__1=Mike343
AUTH__ALLOWEDUSERS__2=JordanRhode
```

## AUTH__EMAILADDRESSDOMAINS__{n}

0-based list of email address domains to allow. If provided, all other usernames
will be denied access to git-related APIs.

For example:

```
AUTH__EMAILADDRESSDOMAINS__0=principlestudios.com
```

## COMMAND__CACHE__DEFAULTENABLED

Default: _true_.

Specifies whether the command cache is enabled. This greatly improves response
times but also greatly increases memory usage.

```
COMMAND__CACHE__DEFAULTENABLED=false
```

## COMMAND__CACHE__TYPESETTINGS__{typeName}

Overrides caching of a specific command type, based on the name. Defaults to the
value of `COMMAND__CACHE__DEFAULTENABLED`.

For example:
```
COMMAND__CACHE__TYPESETTINGS__GITUPSTREAMDATA=true
```

# Common Scenarios

## Working locally

```
GIT__WORKINGDIRECTORY=C:\Users\youruser\source\my-project
GIT__GITTOOLSDIRECTORY=C:\Users\youruser\source\scalable-git-branching-tools
```

[git-tools]: https://github.com/PrincipleStudios/scalable-git-branching-tools/
[ms-auth]: https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-register-applications
