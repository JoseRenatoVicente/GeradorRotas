#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/Services/Team/Teams.API/Teams.API.csproj", "src/Services/Team/Teams.API/"]
COPY ["src/Services/Team/Teams.Infra.Data/Teams.Infra.Data.csproj", "src/Services/Team/Teams.Infra.Data/"]
COPY ["src/Services/Team/Teams.Domain/Teams.Domain.csproj", "src/Services/Team/Teams.Domain/"]
COPY ["src/BuildingBlocks/RouteManager.Domain.Core/RouteManager.Domain.Core.csproj", "src/BuildingBlocks/RouteManager.Domain.Core/"]
COPY ["src/BuildingBlocks/RouteManager.WebAPI.Core/RouteManager.WebAPI.Core.csproj", "src/BuildingBlocks/RouteManager.WebAPI.Core/"]
RUN dotnet restore "src/Services/Team/Teams.API/Teams.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Team/Teams.API"
RUN dotnet build "Teams.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Teams.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Teams.API.dll"]
