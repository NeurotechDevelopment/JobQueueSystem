#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build on top of the official ASP.NET image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Set the user to the non-root app user provided by the base image
USER $APP_UID
# Change the working directory to /app inside the container
WORKDIR /app
# Declare 80 and 443 ports for container to listen at runtime
EXPOSE 8000
# EXPOSE 8443

# Use the SDK image, which includes the compiler and build tools.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Change the working directory to /src inside the container
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restore NuGets for the projects
COPY ["JobRepositoryService/JobRepositoryService.csproj", "JobRepositoryService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "JobRepositoryService/JobRepositoryService.csproj"

# Build the service project with copied dependent projects
COPY ["JobRepositoryService/", "JobRepositoryService/"]
COPY ["Shared/", "Shared/"]
COPY ["Contracts/", "Contracts/"]
WORKDIR "/src/JobRepositoryService"
RUN dotnet build "./JobRepositoryService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
# Publish the application to the /app/publish folder in the image
# /p:UseAppHost=false -> Prevents generating an OS-specific executable wrapper (saves space, cross-platform).
RUN dotnet publish "JobRepositoryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# We start from the minimal runtime image again (no SDK).
FROM base AS final
# Change the working directory to /app inside the final container
WORKDIR /app
# Copy the published application from the publish stage to the final image
COPY --from=publish /app/publish .
# Let Kestrel know it should listen on port 8000 from all interfaces (not just localhost from inside the container). Without this line, the container will not respond to requests from outside.
# No https for dev. For prod use nginx ;https://+:8081
ENV ASPNETCORE_URLS=http://+:8000
# Define what command runs when the container starts
ENTRYPOINT ["dotnet", "JobRepositoryService.dll"]