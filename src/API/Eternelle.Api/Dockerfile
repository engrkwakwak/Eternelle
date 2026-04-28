# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/API/Eternelle.Api/Eternelle.Api.csproj", "src/API/Eternelle.Api/"]
COPY ["src/Modules/Weddings/Eternelle.Modules.Weddings.Infrastructure/Eternelle.Modules.Weddings.Infrastructure.csproj", "src/Modules/Weddings/Eternelle.Modules.Weddings.Infrastructure/"]
COPY ["src/Modules/Weddings/Eternelle.Modules.Weddings.Application/Eternelle.Modules.Weddings.Application.csproj", "src/Modules/Weddings/Eternelle.Modules.Weddings.Application/"]
COPY ["src/Modules/Weddings/Eternelle.Modules.Weddings.Domain/Eternelle.Modules.Weddings.Domain.csproj", "src/Modules/Weddings/Eternelle.Modules.Weddings.Domain/"]
COPY ["src/Modules/Weddings/Eternelle.Modules.Weddings.Presentation/Eternelle.Modules.Weddings.Presentation.csproj", "src/Modules/Weddings/Eternelle.Modules.Weddings.Presentation/"]
COPY ["src/Common/Eternelle.Common.Infrastructure/Eternelle.Common.Infrastructure.csproj", "src/Common/Eternelle.Common.Infrastructure/"]
COPY ["src/Common/Eternelle.Common.Application/Eternelle.Common.Application.csproj", "src/Common/Eternelle.Common.Application/"]
COPY ["src/Common/Eternelle.Common.Domain/Eternelle.Common.Domain.csproj", "src/Common/Eternelle.Common.Domain/"]
COPY ["src/Common/Eternelle.Common.Presentation/Eternelle.Common.Presentation.csproj", "src/Common/Eternelle.Common.Presentation/"]
RUN dotnet restore "./src/API/Eternelle.Api/Eternelle.Api.csproj"
COPY . .
WORKDIR "/src/src/API/Eternelle.Api"
RUN dotnet build "./Eternelle.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Eternelle.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eternelle.Api.dll"]