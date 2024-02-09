# using `jammy` because https://ppa.launchpadcontent.net/git-core/ppa/ubuntu/dists/ does not support dotnet's default (bookworm)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
EXPOSE 80

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
ENV ASPNETCORE_HTTP_PORTS=80

RUN git clone https://github.com/PrincipleStudios/scalable-git-branching-tools.git /git-tools

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-dotnet
WORKDIR /src

COPY ["./Server/Server.csproj", "./Server/"]
COPY ["./Directory.Build.*", "./"]
COPY ["./eng/", "./eng/"]
RUN dotnet build "Server/Server.csproj" -c Release -t:Restore

COPY ["./schemas/", "./schemas/"]
COPY ["./Server/", "./Server/"]

RUN dotnet publish "Server/Server.csproj" -c Release

FROM ubuntu:focal AS build-ui
WORKDIR /src
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       wget \
       ca-certificates \
    \
    # Install Microsoft package feed
    && wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    \
    && apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
      # Add .NET 7 for OpenAPI generators
      dotnet-runtime-7.0 \
      # Add .NET 8 for Build tooling
      dotnet-sdk-8.0 \
    && apt-get clean

# install pnpm
ENV PNPM_HOME=/root/.local/share/pnpm
ENV PNPM_VERSION=v8.10.5
ENV PATH=$PATH:$PNPM_HOME
RUN wget "https://github.com/pnpm/pnpm/releases/download/${PNPM_VERSION}/pnpm-linuxstatic-x64" -O /bin/pnpm \
    && chmod +x /bin/pnpm

COPY ["./.npmrc", "./"]
COPY ["./.nvmrc", "./"]
COPY ["./package.json", "./"]
COPY ["./pnpm-*", "./"]
COPY ["./Directory.Build.*", "./"]
COPY ["./eng/", "./eng/"]
COPY ["./ui/Ui.esproj", "./ui/"]
COPY ["./ui/package.json", "./ui/"]
RUN cd ./ui/ && dotnet build -c Release -t:Restore

COPY ./schemas/ ./schemas/
COPY ./ui/ ./ui/

ARG GITHASH
ENV VITE_GITHASH=${GITHASH}
RUN cd ./ui/ && dotnet build -c Release

WORKDIR /src/Server/wwwroot
RUN find . -type f -not -regex ".*\.\(avif\|jpg\|jpeg\|gif\|png\|webp\|mp4\|webm\)" -exec gzip -k "{}" \; -exec brotli -k "{}" \;


FROM base as final
ARG GITHASH
ENV BUILD__GITHASH=${GITHASH}
ARG BUILDTAG
ENV BUILD__TAG=${BUILDTAG}
COPY --from=build-dotnet /src/artifacts/bin/Server/Release/net8.0/publish .
COPY --from=build-ui /src/Server/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "PrincipleStudios.ScaledGitApp.Server.dll"]
