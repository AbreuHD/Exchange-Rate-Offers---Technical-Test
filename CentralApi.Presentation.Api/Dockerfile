# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY CentralApi.Presentation.Api/CentralApi.Presentation.Api.csproj CentralApi.Presentation.Api/
COPY CentralApi.Core.Application/CentralApi.Core.Application.csproj CentralApi.Core.Application/
COPY CentralApi.Core.Domain/CentralApi.Core.Domain.csproj CentralApi.Core.Domain/
COPY CentralApi.Infrastructure.ExternalApis/CentralApi.Infrastructure.ExternalApis.csproj CentralApi.Infrastructure.ExternalApis/

# Restore all dependencies
RUN dotnet restore "CentralApi.Presentation.Api/CentralApi.Presentation.Api.csproj"

# Copy the rest of the files
COPY CentralApi.Presentation.Api/ CentralApi.Presentation.Api/
COPY CentralApi.Core.Application/ CentralApi.Core.Application/
COPY CentralApi.Core.Domain/ CentralApi.Core.Domain/
COPY CentralApi.Infrastructure.ExternalApis/ CentralApi.Infrastructure.ExternalApis/

# Build the application
WORKDIR /src/CentralApi.Presentation.Api
RUN dotnet build "CentralApi.Presentation.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CentralApi.Presentation.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CentralApi.Presentation.Api.dll"]

