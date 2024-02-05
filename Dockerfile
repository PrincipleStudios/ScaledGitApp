# using `jammy` because https://ppa.launchpadcontent.net/git-core/ppa/ubuntu/dists/ does not support dotnet's default (bookworm)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
RUN apt update \
    # Install Microsoft's repo
    && apt install -y wget ca-certificates gnupg \
    && wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    # Prerequisites to get the latest git
    && apt-get install -y apt-transport-https software-properties-common python3-launchpadlib \
    && add-apt-repository ppa:git-core/ppa \
    # Start installing actual runtime dependencies
    && apt update \
    && apt install -y \
        # install git
        git \
        # powershell is necessary because the git tools are in powershell
        powershell \
    && rm -rf /var/lib/apt/lists/*

RUN git clone https://github.com/PrincipleStudios/scalable-git-branching-tools.git /git-tools

FROM base as final
ARG GITHASH
ENV BUILD__GITHASH=${GITHASH}

# TODO - add entrypoint
# ENTRYPOINT ["dotnet", "ScaledGitApp.Server.dll"]