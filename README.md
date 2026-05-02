# Greater Cairo Transportation Network Optimization

A comprehensive smart city transportation management system built for the **CSE112 Algorithms and Data Structures** practical project. This system applies advanced algorithmic techniques to solve real-world urban routing, scheduling, and infrastructure challenges in the Greater Cairo metropolitan area.

## 🌟 Project Features & Algorithms Implemented

This project successfully fulfills all course requirements by integrating the following algorithms into a unified, interactive platform:

1.  **Dijkstra's Algorithm (Graph Search):** Standard shortest path routing.
2.  **A\* Search (Heuristic Search):** Optimized emergency vehicle routing to medical facilities using Euclidean distance heuristics.
3.  **Time-Varying Dijkstra (Traffic-Aware):** Dynamic routing that adapts to morning, evening, and night traffic multipliers and live road congestion.
4.  **Prim's Algorithm (Minimum Spanning Tree):** Designs a cost-efficient urban network expansion plan, prioritizing high-population areas and critical facilities.
5.  **0/1 Knapsack (Dynamic Programming):** Allocates a finite road maintenance budget to the most critical roads to maximize priority utility.
6.  **Bounded Knapsack (Dynamic Programming):** Schedules and allocates transit vehicles across metro and bus lines to maximize passenger coverage.
7.  **Greedy Algorithm (Traffic Signals):** Real-time optimization of intersection green-light cycles based on live traffic flow.
8.  **ML Predictions (Gradient Boosting):** Traffic congestion forecasting using machine learning model (R²=0.94) trained on historical data for road congestion prediction.
9.  **Simulation Framework:** Supports real-time road closures (accidents), weather penalties (Rain/Storm), and emergency preemption.

---

## 🏗️ System Architecture

The project is divided into two main components:

- **Backend:** ASP.NET Core 10 REST API with Entity Framework Core and SQLite.
- **Frontend:** Next.js 16 (React 19) web application with interactive Leaflet mapping.

![System Architecture Diagram](Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/Architecture%20-%20Component%20Diagram.png)

**Figure:** Component Architecture showing frontend, backend services, and data layer

---

## 🚀 Getting Started

You can run the entire system using either Docker (recommended) or locally with .NET and Node.js.

### Option 1: Docker Compose (Recommended)

The easiest way to start the system is using Docker Compose, which spins up both the Backend and Frontend in a single command.

1.  **Navigate to the Apps directory:**
    ```bash
    cd Apps
    ```
2.  **Start the containers:**
    ```bash
    docker compose up -d
    ```
3.  **Access the applications:**
    - **Frontend UI:** `http://localhost:3000`
    - **Backend API:** `http://localhost:8080` (with Swagger at `http://localhost:8080/swagger`)

### Option 2: Local Manual Setup

#### 1. Running the Backend API

The database is automatically created and seeded with Cairo geographical and traffic data on the first run.

1.  Navigate to the server directory:
    ```bash
    cd Apps/Server/CairoTransportation
    ```
2.  Run the application:
    ```bash
    dotnet run
    ```
3.  The API will start (usually on `http://localhost:5000`).

#### 2. Running the Frontend UI

1.  Navigate to the client directory:
    ```bash
    cd Apps/client
    ```
2.  Install dependencies and start:
    ```bash
    npm install
    ```
3.  Start the development server:
    ```bash
    npm run dev
    ```
4.  Open `http://localhost:3000`.

---

## ☁️ Deployment

The application is deployed to a production VPS with automated CI/CD via GitHub Actions.

- **Frontend:** https://gcts.abosaleh.site
- **API:** https://gcts-api.abosaleh.site

![Deployment Diagram - CI/CD Pipeline](Apps/Server/CairoTransportation/Docs/DIAGRAMS/PlantUMLout/Deployment%20Diagram%20-%20CI-CD%20Pipeline.png)

**Figure:** Production deployment architecture with GitHub Actions, Docker, and Cloudflare tunnel

### Auto-Deploy Setup

The project uses GitHub Actions to automatically deploy on every push to `main`:

1.  **Configure GitHub Secrets:**
    - `VPS_HOST` - Server IP/hostname
    - `VPS_USER` - SSH username
    - `VPS_SSH_KEY` - Private SSH key

2.  **Workflow:** `.github/workflows/deploy.yml` triggers on push to main, SSHs into VPS, pulls latest code, rebuilds Docker containers.

### Local Production Build

To build for production locally:

```bash
cd Apps
docker compose -f docker-compose.prod.yml up -d --build
```

---

## 📚 Documentation & Testing

- **`REPORT.md` (and `REPORT.pdf`)**: Contains the comprehensive technical report detailing system architecture, algorithm pseudocode, complexity analysis, and mathematical justifications.
- **`TESTING.md`**: Provides a step-by-step guide and specific scenarios to manually test and demonstrate every algorithmic feature of the system via the UI.
- **`project_audit.md`**: A detailed compliance matrix linking project requirements to their implementations.

---

## 🗺️ Seed Data Overview

The system includes a robust, pre-seeded SQLite database (`TablesData.sql`) representing Greater Cairo:

- **35 Locations:** 21 residential neighborhoods and 14 critical facilities (hospitals, fire stations).
- **74 Roads:** 53 existing roads and 21 potential new roads for MST expansion.
- **Traffic Data:** Flow capacities and period multipliers (Morning, Evening, Night).
- **Transit:** 4 Metro lines and 4 major Bus routes.
- **Maintenance:** 10 prioritized road segments requiring repair.

## 👥 Team

**AIU-SoftWave**
