# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy everything (solution, projects, etc.)
COPY . .

WORKDIR /app/src/API/[ProjectName]

# Restore dependencies
RUN dotnet restore [ProjectName].csproj

# Build and publish
RUN dotnet publish [ProjectName].csproj -c Release -o ./out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/src/API/[ProjectName]/out ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENTRYPOINT ["dotnet", "[ProjectName].dll"]
EXPOSE 80
