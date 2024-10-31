FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Bootstrapper/Api/Api.csproj", "Bootstrapper/Api/"]
COPY ["Modules/Basket/Basket.csproj", "Modules/Basket/"]
COPY ["Shared/Shared/Shared.csproj", "Shared/Shared/"]
COPY ["Shared/Shared.Contracts/Shared.Contracts.csproj", "Shared/Shared.Contracts/"]
COPY ["Modules/Catalog.Contracts/Catalog.Contracts.csproj", "Modules/Catalog.Contracts/"]
COPY ["Shared/Shared.Messaging/Shared.Messaging.csproj", "Shared/Shared.Messaging/"]
COPY ["Modules/Catalog/Catalog.csproj", "Modules/Catalog/"]
COPY ["Modules/Ordering/Ordering.csproj", "Modules/Ordering/"]
RUN dotnet restore "Bootstrapper/Api/Api.csproj"
COPY . .
WORKDIR "/src/Bootstrapper/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
