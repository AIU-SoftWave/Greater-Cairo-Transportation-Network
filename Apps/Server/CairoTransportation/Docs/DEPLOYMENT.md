# Deployment Guide

## Production URLs

- **Frontend:** https://gcts.abosaleh.site
- **API:** https://gcts-api.abosaleh.site

Access is provided through Cloudflare Tunnel (cloudflared), which creates secure HTTPS tunnels to the internal Docker network without requiring traditional port forwarding.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     Cloudflare Tunnel                       │
│                   (cloudflared container)                   │
└─────────────────────────────────────────────────────────────┘
                          │
            ┌─────────────┴─────────────┐
            ▼                               ▼
   ┌─────────────────┐           ┌─────────────────┐
   │   Next.js UI    │           │  .NET 10 API    │
   │   (Port 3000)   │           │   (Port 8080)   │
   └─────────────────┘           └─────────────────┘
            │                               │
            └───────────┬───────────────────┘
                        ▼
            ┌─────────────────────────┐
            │   Internal Bridge Net    │
            │    (cairo-internal)      │
            └─────────────────────────┘
```

---

## CI/CD Pipeline

### GitHub Actions Workflow

The auto-deployment workflow is defined in `.github/workflows/deploy.yml` and triggers automatically on every push to the `main` branch:

```yaml
name: Auto Deploy to VPS

on:
  push:
    branches:
      - main

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      - name: Deploy to VPS via SSH
        uses: appleboy/ssh-action@v0.1.10
        with:
          host: ${{ secrets.VPS_HOST }}
          username: ${{ secrets.VPS_USER }}
          key: ${{ secrets.VPS_SSH_KEY }}
          script: |
            cd /home/abosaleh/MyProjects/Greater-Cairo-Transportation-Network
            git fetch origin
            git reset --hard origin/main
            docker compose -f Apps/docker-compose.prod.yml up -d --build
```

### Required GitHub Secrets

Configure these secrets in your GitHub repository settings (Settings → Secrets and variables → Actions):

| Secret | Description |
|--------|-------------|
| `VPS_HOST` | Server hostname or IP address |
| `VPS_USER` | SSH username for deployment |
| `VPS_SSH_KEY` | Private SSH key for authentication |

### Deployment Process

1. **Code Push**: Developer pushes changes to `main` branch
2. **GitHub Actions Trigger**: Workflow starts automatically
3. **SSH Connection**: Actions connect to VPS using stored credentials
4. **Code Update**: VPS pulls latest code from GitHub
5. **Container Rebuild**: Docker rebuilds images with new code
6. **Service Restart**: Containers are recreated with new images
7. **Health Check**: Deployment verifies both services are running

---

## Docker Production Setup

### Docker Compose Configuration

The production deployment uses Docker Compose to orchestrate multiple containers:

```yaml
# Apps/docker-compose.prod.yml
name: cairo-transportation

services:
  client:
    build:
      context: ./client
      dockerfile: Dockerfile
      args:
        NEXT_PUBLIC_API_BASE_URL: https://gcts-api.abosaleh.site
    networks:
      - cloudflared-net
      - cairo-internal

  api:
    build:
      context: ./Server/CairoTransportation
      dockerfile: Dockerfile
    networks:
      - cloudflared-net
      - cairo-internal

networks:
  cloudflared-net:
    external: true
  cairo-internal:
    driver: bridge
```

### Backend Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CairoTransportation.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CairoTransportation.dll"]
```

### Frontend Dockerfile

```dockerfile
FROM node:20-alpine AS builder
ARG NEXT_PUBLIC_API_BASE_URL
RUN npm install -g pnpm
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN pnpm install
COPY . .
ENV NEXT_PUBLIC_API_BASE_URL=$NEXT_PUBLIC_API_BASE_URL
RUN pnpm build

FROM node:20-alpine
WORKDIR /app
ARG NEXT_PUBLIC_API_BASE_URL
ENV NEXT_PUBLIC_API_BASE_URL=$NEXT_PUBLIC_API_BASE_URL
ENV NODE_ENV=production
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./package.json
CMD ["node", "node_modules/next/dist/bin/next", "start"]
```

### Network Configuration

- **cloudflared-net**: External network connected to Cloudflare tunnel for HTTPS access
- **cairo-internal**: Internal bridge network for container-to-container communication

---

## Manual Deployment

To deploy manually without GitHub Actions:

1. SSH into the VPS:
   ```bash
   ssh user@vps-host
   ```

2. Navigate to project directory:
   ```bash
   cd /home/user/MyProjects/Greater-Cairo-Transportation-Network
   ```

3. Pull latest changes:
   ```bash
   git fetch origin
   git reset --hard origin/main
   ```

4. Build and start containers:
   ```bash
   cd Apps
   docker compose -f docker-compose.prod.yml up -d --build
   ```

5. View logs:
   ```bash
   docker compose -f docker-compose.prod.yml logs -f
   ```

---

## Troubleshooting

### Check Container Status
```bash
docker ps
```

### View Logs
```bash
docker compose -f docker-compose.prod.yml logs api
docker compose -f docker-compose.prod.yml logs client
```

### Restart Services
```bash
docker compose -f docker-compose.prod.yml restart
```

### Rebuild Specific Service
```bash
docker compose -f docker-compose.prod.yml up -d --build api
docker compose -f docker-compose.prod.yml up -d --build client
```

---

## Machine Learning Predictions

The system includes ML-based traffic congestion predictions:

- **Model**: Gradient Boosting Regressor
- **Training Data**: 722 records from predictions.json
- **Accuracy**: R² = 0.94
- **Prediction Scale**: 0 (low congestion) to 2 (high congestion)

The ML predictions can be enabled in the Time-Varying Dijkstra routing options to incorporate forecasted congestion into pathfinding.