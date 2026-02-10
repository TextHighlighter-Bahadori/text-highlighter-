FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/api-gateway/ApiGateway/ApiGateway.csproj", "api-gateway/ApiGateway/"]
RUN dotnet restore "api-gateway/ApiGateway/ApiGateway.csproj"

COPY src/api-gateway/ApiGateway api-gateway/ApiGateway
WORKDIR "/src/api-gateway/ApiGateway"
RUN dotnet build "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY src/api-gateway/ApiGateway/ocelot.json ./ocelot.json   
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
