FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["Modules/Dinners/Application/Dinners.Application.csproj", "Modules/Dinners/Application/"]
COPY ["BuildingBlocks/Application/BuildingBlocks.Application.csproj", "BuildingBlocks/Application/"]
COPY ["BuildingBlocks/Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks/Domain/"]
COPY ["Modules/Dinners/Domain/Dinners.Domain.csproj", "Modules/Dinners/Domain/"]
COPY ["Modules/Dinners/IntegrationEvents/Dinners.IntegrationEvents.csproj", "Modules/Dinners/IntegrationEvents/"]
COPY ["Modules/Dinners/Infrastructure/Dinners.Infrastructure.csproj", "Modules/Dinners/Infrastructure/"]
COPY ["Modules/Users/Application/Users.Application.csproj", "Modules/Users/Application/"]
COPY ["Modules/Users/Domain/Users.Domain.csproj", "Modules/Users/Domain/"]
COPY ["Modules/Users/Infrastructure/Users.Infrastructure.csproj", "Modules/Users/Infrastructure/"]
RUN dotnet restore "./API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]