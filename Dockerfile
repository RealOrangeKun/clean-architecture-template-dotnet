# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything (solution, projects, etc.)
COPY . .

WORKDIR /src/src/API/[ProjectName].Api

# Restore dependencies
RUN dotnet restore [ProjectName].Api.csproj

# Build and publish
RUN dotnet publish [ProjectName].Api.csproj -c Release -o ./out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /src/src/API/[ProjectName].Api/out ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENTRYPOINT ["dotnet", "[ProjectName].Api.dll"]
EXPOSE 80
