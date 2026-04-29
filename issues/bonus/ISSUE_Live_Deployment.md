# Issue: Deploy Demo as Live Web App

## Category

Deployment & Engineering (1.5 Marks of 3 total)

## Priority

High

## Status

Open

---

## Requirement

Deploy the demo as a live web app (GitHub Pages, Vercel, or Render) with a shareable link.

---

## Current State

- ✅ Docker containerization is complete (`docker-compose.yml` + Dockerfiles)
- ❌ No live deployment exists

---

## Implementation Plan

### Option A: Render (Recommended — supports both services)

#### 1. Create `render.yaml`

- [ ] Add `render.yaml` at project root with:
  - API service: Docker deployment from `Apps/Server/CairoTransportation/Dockerfile`
  - Client service: Docker deployment from `Apps/client/Dockerfile`
- [ ] Set environment variables:
  - API: `ASPNETCORE_URLS=http://+:80`
  - Client: `NEXT_PUBLIC_API_BASE_URL=https://<api-service>.onrender.com`

#### 2. Push to GitHub

- [ ] Ensure project is pushed to GitHub
- [ ] Connect GitHub repo to Render dashboard
- [ ] Render auto-detects `render.yaml`

#### 3. Configure CORS

- [ ] Add Render client URL to CORS origins in `Program.cs`
- [ ] Add `https://<client-service>.onrender.com` to allowed origins

---

### Option B: Vercel (Frontend) + Render (Backend)

#### 1. Deploy Backend to Render

- [ ] Create Render web service from Docker
- [ ] Note the API URL (e.g., `https://cairo-transport-api.onrender.com`)

#### 2. Deploy Frontend to Vercel

- [ ] Connect GitHub repo to Vercel
- [ ] Set root directory to `Apps/client`
- [ ] Set build env: `NEXT_PUBLIC_API_BASE_URL=https://cairo-transport-api.onrender.com`
- [ ] Deploy

---

### Option C: Railway (Simplest Docker Compose support)

#### 1. Connect GitHub to Railway

- [ ] Railway natively supports `docker-compose.yml`
- [ ] Auto-deploys both services
- [ ] Provides shareable URLs for each service

---

### Option D: VPS + Cloudflare Tunnel (User's Setup)

You have a VPS with a domain already pointing to it. Use Cloudflare Tunnel (`cloudflared`) to expose the Docker Compose services without opening firewall ports or managing DNS.

#### 1. Add Cloudflare Tunnel to Docker Compose

- [ ] Create `tunnel` service in `docker-compose.yml`:

```yaml
tunnel:
  image: cloudflare/cloudflared:latest
  command: tunnel --no-autoupdate run --token ${TUNNEL_TOKEN}
  env_file:
    - .env
  restart: unless-stopped
  networks:
    - cairo-network
  depends_on:
    - api
    - client
```

#### 2. Create Tunnel in Cloudflare Dashboard

- [ ] Go to Cloudflare Zero Trust → Networks → Tunnels
- [ ] Create a new tunnel (type: Cloudflared)
- [ ] Copy the tunnel token

#### 3. Configure .env

- [ ] Add `TUNNEL_TOKEN=<your-tunnel-token>` to `.env` file (never commit this)

#### 4. Configure Public Hostnames

- [ ] Add public hostname: `transport.yourdomain.com` → `http://client:3000`
- [ ] Add public hostname: `api.yourdomain.com` → `http://api:80`
- Or use subpaths: `yourdomain.com` (client) and `yourdomain.com/api` → `http://api:80`

#### 5. Update CORS

- [ ] Add `https://yourdomain.com` to CORS origins in `Program.cs`

#### 6. Deploy

- [ ] `docker compose up -d` on your VPS
- [ ] Tunnel connects outbound (no firewall rules needed)
- [ ] Access via `https://yourdomain.com`

**Pros:**

- No port forwarding or firewall changes
- HTTPS + DDoS protection via Cloudflare
- Your VPS, but with Cloudflare's edge caching
- Free tier covers personal/demo use

---

## Post-Deployment

- [ ] Test live URL — verify map loads, algorithms work
- [ ] Add live demo URL to `README.md`
- [ ] Screenshot the live app for LinkedIn post

---

## Acceptance Criteria

- [ ] Live URL accessible from any browser
- [ ] Frontend loads Cairo map with network topology
- [ ] At least one algorithm (Dijkstra) works end-to-end on live deployment
- [ ] Demo URL added to README
