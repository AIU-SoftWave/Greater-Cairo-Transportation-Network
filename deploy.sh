#!/bin/bash
set -e

# Configuration
REGISTRY="ghcr.io"
IMAGE_NAME="greater-cairo-transportation-network"
COMPOSE_FILE="docker-compose.yml"

echo "=== Deploying Cairo Transportation Network ==="

# Login to container registry
echo "Logging in to GitHub Container Registry..."
echo "$GH_TOKEN" | docker login $REGISTRY -u ${{ github.actor }} --password-stdin

# Pull latest images
echo "Pulling latest images..."
docker pull $REGISTRY/$IMAGE_NAME/server:latest
docker pull $REGISTRY/$IMAGE_NAME/client:latest

# Create docker-compose file on VPS
cat > $COMPOSE_FILE << 'EOF'
version: '3.8'

services:
  server:
    image: ghcr.io/aiu-softwave/greater-cairo-transportation-network/server:latest
    containerName: cairo-server
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    restart: unless-stopped
    networks:
      - cairo-network

  client:
    image: ghcr.io/aiu-softwave/greater-cairo-transportation-network/client:latest
    containerName: cairo-client
    ports:
      - "3000:3000"
    environment:
      - NEXT_PUBLIC_API_BASE_URL=http://server:8080
    depends_on:
      - server
    restart: unless-stopped
    networks:
      - cairo-network

networks:
  cairo-network:
    driver: bridge
EOF

# Stop and remove old containers
echo "Stopping old containers..."
docker compose -f $COMPOSE_FILE down || true

# Start new containers
echo "Starting services..."
docker compose -f $COMPOSE_FILE up -d

# Show status
echo ""
echo "=== Deployment Complete ==="
docker compose -f $COMPOSE_FILE ps

echo ""
echo "Services available at:"
echo "  - Client: http://localhost:3000"
echo "  - API: http://localhost:8080"