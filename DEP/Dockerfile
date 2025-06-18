# Use the .NET 6 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet publish DEP.csproj -c Release -o out

# Use the ASP.NET runtime to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out ./

# Start the app
ENTRYPOINT ["dotnet", "DEP.dll"]
