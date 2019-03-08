FROM microsoft/dotnet:3.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:3.0-sdk AS build
WORKDIR /src
COPY OpenApiCleaner.csproj
RUN dotnet restore /OpenApiCleaner.csproj
COPY . .
WORKDIR /src/
RUN dotnet build OpenApiCleaner.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish OpenApiCleaner.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OpenApiCleaner.dll"]
