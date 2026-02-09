#!/bin/bash

# Configuration
NETWORK="topcv-be_topcv-network"
SA_PASSWORD=$(grep SA_PASSWORD .env | cut -d '=' -f2)

if [ -z "$SA_PASSWORD" ]; then
    echo "SA_PASSWORD not found in .env file. using default."
    SA_PASSWORD="TopCv@2024!Strong"
fi

echo "Running migrations using Docker SDK container..."

docker run --rm \
    --network $NETWORK \
    -v $(pwd):/src \
    -w /src \
    -e "ConnectionStrings__DefaultConnection=Server=sqlserver;Database=topCv;User Id=sa;Password=$SA_PASSWORD;TrustServerCertificate=True;" \
    mcr.microsoft.com/dotnet/sdk:8.0 \
    sh -c "dotnet tool install --global dotnet-ef && \
           export PATH=\"\$PATH:/root/.dotnet/tools\" && \
           dotnet ef database update --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api --no-build"

if [ $? -eq 0 ]; then
    echo "Migrations applied successfully!"
else
    echo "Failed to apply migrations."
    exit 1
fi
