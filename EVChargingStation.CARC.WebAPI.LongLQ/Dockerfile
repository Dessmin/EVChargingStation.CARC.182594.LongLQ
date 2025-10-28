# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["EVChargingStation.CARC.WebAPI.LongLQ/EVChargingStation.CARC.WebAPI.LongLQ.csproj", "EVChargingStation.CARC.WebAPI.LongLQ/"]
COPY ["EVChargingStation.CARC.Application.LongLQ/EVChargingStation.CARC.Application.LongLQ.csproj", "EVChargingStation.CARC.Application.LongLQ/"]
COPY ["EVChargingStation.CARC.Infrastructure.LongLQ/EVChargingStation.CARC.Infrastructure.LongLQ.csproj", "EVChargingStation.CARC.Infrastructure.LongLQ/"]
COPY ["EVChargingStation.CARC.Domain.LongLQ/EVChargingStation.CARC.Domain.LongLQ.csproj", "EVChargingStation.CARC.Domain.LongLQ/"]
RUN dotnet restore "./EVChargingStation.CARC.WebAPI.LongLQ/EVChargingStation.CARC.WebAPI.LongLQ.csproj"
COPY . .
WORKDIR "/src/EVChargingStation.CARC.WebAPI.LongLQ"
RUN dotnet build "./EVChargingStation.CARC.WebAPI.LongLQ.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EVChargingStation.CARC.WebAPI.LongLQ.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EVChargingStation.CARC.WebAPI.LongLQ.dll"]