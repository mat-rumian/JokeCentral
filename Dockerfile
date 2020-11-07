FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

COPY OpenTelemetryExtensions/*.csproj OpenTelemetryExtensions/
COPY JokeCentral.Api/*.csproj JokeCentral.Api/
COPY JokeCentral/*.csproj JokeCentral/
RUN dotnet restore JokeCentral/JokeCentral.csproj

COPY OpenTelemetryExtensions/ OpenTelemetryExtensions/
COPY JokeCentral.Api/ JokeCentral.Api/
COPY JokeCentral/ JokeCentral/
WORKDIR /source/JokeCentral
RUN dotnet build -c release --no-restore

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JokeCentral.dll"]
