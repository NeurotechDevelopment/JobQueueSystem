# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# Use the SDK image, which includes the compiler and build tools.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restore NuGets for the projects
COPY ["JobHandlers/JobHandlers.csproj", "JobHandlers/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "JobHandlers/JobHandlers.csproj"

# Build the service project with copied dependent projects
COPY ["JobHandlers/", "JobHandlers/"]
COPY ["Shared/", "Shared/"]
COPY ["Contracts/", "Contracts/"]
WORKDIR "/src/JobHandlers"
RUN dotnet build "./JobHandlers.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobHandlers.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobHandlers.dll"]