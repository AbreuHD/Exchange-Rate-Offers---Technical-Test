# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ThirdApi.Presentation/ThirdApi.Presentation.csproj ThirdApi.Presentation/
RUN dotnet restore "ThirdApi.Presentation/ThirdApi.Presentation.csproj"

# Copy the rest of the files
COPY ThirdApi.Presentation/ ThirdApi.Presentation/

# Build the application
WORKDIR /src/ThirdApi.Presentation
RUN dotnet build "ThirdApi.Presentation.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "ThirdApi.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
COPY ThirdApi.Presentation/IsoCodes.xml .
ENTRYPOINT ["dotnet", "ThirdApi.Presentation.dll"]

