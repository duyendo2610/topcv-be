# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY topCv/topCv.slnx .
COPY topCv/topCv.Api/topCv.Api.csproj topCv.Api/
COPY topCv/topCv.Application/topCv.Application.csproj topCv.Application/
COPY topCv/topCv.Domain/topCv.Domain.csproj topCv.Domain/
COPY topCv/topCv.Infrastructure/topCv.Infrastructure.csproj topCv.Infrastructure/

# Restore dependencies
RUN dotnet restore topCv.Api/topCv.Api.csproj

# Copy the rest of the source code
COPY topCv/ .

# Build the application
RUN dotnet build topCv.Api/topCv.Api.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish topCv.Api/topCv.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final stage - runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health check
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --gid 1001 appuser


# Create directory for file uploads
RUN mkdir -p /app/uploads && \
    chown -R appuser:appgroup /app/uploads

# Copy published files
COPY --from=publish /app/publish .

# Change ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Configure environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "topCv.Api.dll"]
