# Build the API. Run from the repository root:
#   docker build -f docker/api.Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:{{DotnetSdkImageTag}} AS build
WORKDIR /source

# Copy project files first so 'restore' is cached independently of source edits.
COPY Directory.Build.props ./
COPY src/Backend/{{ProjectName}}.Domain/*.csproj src/Backend/{{ProjectName}}.Domain/
COPY src/Backend/{{ProjectName}}.Application/*.csproj src/Backend/{{ProjectName}}.Application/
COPY src/Backend/{{ProjectName}}.Infrastructure/*.csproj src/Backend/{{ProjectName}}.Infrastructure/
COPY src/Backend/{{ProjectName}}.Api/*.csproj src/Backend/{{ProjectName}}.Api/
RUN dotnet restore src/Backend/{{ProjectName}}.Api/{{ProjectName}}.Api.csproj

COPY src/Backend/ src/Backend/
RUN dotnet publish src/Backend/{{ProjectName}}.Api/{{ProjectName}}.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app

FROM mcr.microsoft.com/dotnet/aspnet:{{DotnetRuntimeImageTag}} AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS={{ApiHttpPort}}
EXPOSE {{ApiHttpPort}}

# Run as the non-root user the base image provides.
USER $APP_UID

COPY --from=build /app ./
ENTRYPOINT ["dotnet", "{{ProjectName}}.Api.dll"]
