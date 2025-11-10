# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY FirstApi.Presentation/FirstApi.Presentation.csproj FirstApi.Presentation/
RUN dotnet restore "FirstApi.Presentation/FirstApi.Presentation.csproj"

# Copy the rest of the files
COPY FirstApi.Presentation/ FirstApi.Presentation/

# Build the application
WORKDIR /src/FirstApi.Presentation
RUN dotnet build "FirstApi.Presentation.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FirstApi.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
COPY FirstApi.Presentation/IsoCodes.xml .
ENTRYPOINT ["dotnet", "FirstApi.Presentation.dll"]

