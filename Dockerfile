# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all files
COPY . .

# Restore dependencies for the solution (specify path to .sln)
RUN dotnet restore Connections/Connections.sln

# Publish the API project
RUN dotnet publish Connections/Connections.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Run the API
ENTRYPOINT ["dotnet", "Connections.dll"]
