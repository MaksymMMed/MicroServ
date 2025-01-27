FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SenderAPI/SenderAPI.csproj", "SenderAPI/"]
COPY ["../TransitService/TransitService.csproj", "TransitService/"]
RUN dotnet restore "./SenderAPI/SenderAPI.csproj"
COPY . .
WORKDIR "/src/SenderAPI"
RUN dotnet build "./SenderAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SenderAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SenderAPI.dll"]