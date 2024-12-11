#docker tag webapi rgabriel15/insidertradingbr:<my_tag_name>
#docker push rgabriel15/insidertradingbr:<my_tag_name>
#docker ps

# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Expose ports that the app will listen on
EXPOSE 8080
EXPOSE 8081

# Set DOTNET_RUNNING_IN_CONTAINER environment variable (not required on Azure, but required for local development).
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASSETS_PATH="app/Assets/"

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["Directory.Build.props", "."]
COPY ["src/**/*.csproj", "src/"]
COPY ["src/Web.API/Assets", "app/Assets/"]
COPY . .

# Restore and build the project
RUN dotnet restore "src/Web.API/Web.API.csproj"
WORKDIR "/src/src/Web.API"
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "./Web.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "./Web.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.API.dll"]