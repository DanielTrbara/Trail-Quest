# Build-Stage: .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Alles ins Image kopieren
COPY . .

# Restore + Publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime-Stage: schlankes ASP.NET-Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# veröffentlichte App aus dem Build-Stage übernehmen
COPY --from=build /app/publish .

# Render setzt $PORT -> App lauscht darauf
ENV ASPNETCORE_URLS=http://+:$PORT

# Optional, nur Info
EXPOSE 8080

# Name der DLL = dein Projektname (StepUp.csproj -> StepUp.dll)
ENTRYPOINT ["dotnet", "StepUp.dll"]