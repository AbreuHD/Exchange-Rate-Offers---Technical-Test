# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY SecondApi.Presentation/SecondApi.Presentation.csproj SecondApi.Presentation/
RUN dotnet restore "SecondApi.Presentation/SecondApi.Presentation.csproj"

# Copy the rest of the files
COPY SecondApi.Presentation/ SecondApi.Presentation/

# Build the application
WORKDIR /src/SecondApi.Presentation
RUN dotnet build "SecondApi.Presentation.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SecondApi.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecondApi.Presentation.dll"]

