FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY src/services/FortranHighlighter/FortranHighlighter.csproj services/FortranHighlighter/
RUN dotnet restore services/FortranHighlighter/FortranHighlighter.csproj

COPY src/services/FortranHighlighter/ services/FortranHighlighter/
WORKDIR /src/services/FortranHighlighter
RUN dotnet build FortranHighlighter.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish FortranHighlighter.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FortranHighlighter.dll"]
