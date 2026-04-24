# Cairo Transportation Network – Project Overview

## Table of Contents
1. [Project Goal](#project-goal)
2. [Current Stack](#current-stack)
3. [Architecture](#architecture)
4. [Repository Layout](#repository-layout)
5. [Documentation Structure](#documentation-structure)
6. [Domain Model](#domain-model)
7. [API Endpoints](#api-endpoints)
8. [Startup, Database, and Seeding](#startup-database-and-seeding)
9. [What Is Implemented](#what-is-implemented)
10. [What Still Needs to Be Implemented](#what-still-needs-to-be-implemented)
11. [Development Notes](#development-notes)
12. [Run Guide](#run-guide)

---

## Project Goal

This project is a **Smart City Transportation Network Optimization** system for the Greater Cairo area.

The goal is to build a clean, maintainable API and data layer that can support the algorithmic work required by the course brief:

- graph/network optimization
- shortest path routing
- emergency routing
- time-dependent traffic analysis
- public transit scheduling
- road maintenance planning
- greedy traffic control strategies

The current codebase focuses on the **data model, API layer, persistence, seeding, and documentation** needed to support those algorithms.

---

## Current Stack

- **Framework:** ASP.NET Core on .NET 10
- **Database:** SQLite
- **ORM:** Entity Framework Core
- **API style:** REST controllers + OpenAPI/Swagger
- **Seeding:** SQL file loaded once when the database is empty
- **Startup schema updates:** EF Core migrations applied automatically on app start

---

## Architecture

The application currently follows a simple **layered API architecture**:

```text
HTTP Request
    │
    ▼
Controllers
    │
    ▼
Services
    │
    ▼
Entity Framework Core
    │
    ▼
SQLite Database
```

### Layers

| Layer | Responsibility |
|---|---|
| Controllers | Expose REST endpoints |
| Services | Query and coordinate business data access |
| Data | EF Core DbContext, migration startup, seeding |
| Models | Database entities |

### Startup Flow

1. Load `DefaultConnection` from configuration.
2. Register controllers and services.
3. Apply EF Core migrations automatically.
4. Seed the database from `Data/TablesData.sql` only if the database is empty.
5. Expose API controllers and OpenAPI/Swagger.

---

## Repository Layout

```text
Apps/Server/CairoTransportation/
├── Controllers/
├── Data/
├── Docs/
├── Migrations/
├── Models/
├── Services/
├── Properties/
├── Program.cs
└── appsettings.json
```

---

## Documentation Structure

The documentation is organized inside `Docs/` so it is easy to edit later.

### Main docs folders
- `Docs/START-HERE/` – beginner orientation
- `Docs/ASP-NET-CORE/` – ASP.NET Core basics
- `Docs/DATA/` – models, schema, EF Core, seeding
- `Docs/API/` – controllers, endpoints, Swagger, responses
- `Docs/ALGORITHMS/` – algorithm behavior and roadmap
- `Docs/DEV/` – libraries, setup, contribution notes
- `Docs/PROJECT/` – project goals and status

### Docs hub
- `Docs/README.md` is the navigation page

---

## Domain Model

The database represents Cairo transportation data through the following entities:

### Location
Represents neighborhoods and facilities.

Key fields:
- `Id`
- `Name`
- `Type`
- `Category`
- `Population`
- `X`, `Y`
- `IsCritical`

### Road
Represents a directed connection between two locations.

Key fields:
- `Id`
- `FromLocationId`
- `ToLocationId`
- `Distance`
- `Capacity`
- `Condition`
- `IsExisting`
- `ConstructionCost`

### TrafficFlow
Represents traffic volume on a road for a time period.

Key fields:
- `RoadId`
- `Period`
- `Flow`

### TransportRoute
Represents metro or bus routes.

Key fields:
- `Id`
- `Type`
- `DailyPassengers`
- `VehiclesAssigned`

### RouteStop
Represents ordered stops for a transport route.

### TransportDemand
Represents origin-destination passenger demand.

### RoadMaintenance
Represents maintenance priority and estimated cost for a road.

---

## API Endpoints

### Locations
- `GET /api/locations`
- `GET /api/locations/{id}`

### Roads
- `GET /api/roads`
- `GET /api/roads/{id}`
- `GET /api/roads/from/{locationId}`
- `GET /api/roads/{roadId}/maintenance`

### Traffic
- `GET /api/traffic/road/{roadId}`
- `GET /api/traffic/period/{period}`

### Routes
- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/{id}/stops`

### Docs
- `GET /swagger` in development
- `GET /openapi/v1.json` in development

---

## Startup, Database, and Seeding

### Database behavior
- EF Core migrations are applied automatically on startup.
- If the SQLite database is empty, the seed script is executed once.
- Seed data lives in `Data/TablesData.sql`.

### Configuration
The active connection string comes from:
- `ConnectionStrings:DefaultConnection`

### Important note
This setup is intentionally minimal:
- no separate repository layer
- no DTO layer yet
- no complex startup orchestration
- migrations are still the source of truth for schema changes

---

## What Is Implemented

### Core platform
- ASP.NET Core API
- EF Core + SQLite
- automatic migration application
- one-time seeding from SQL
- OpenAPI/Swagger UI

### Data access
- locations API
- roads API
- traffic API
- routes API
- road maintenance endpoint

### Persistence
- entity models for all main tables
- EF Core relationship mapping
- seed SQL file with Cairo sample data

---

## What Still Needs to Be Implemented

The following course features are still not implemented as dedicated algorithm services/endpoints:

### 1. Minimum Spanning Tree / Network Design
Need:
- MST algorithm over existing + potential roads
- cost-efficient road network construction
- critical facility connectivity rules
- cost analysis and network result output

### 2. Shortest Path Algorithms
Need:
- Dijkstra route planning
- A* emergency routing
- time-dependent shortest path using traffic flow

### 3. Dynamic Programming
Need:
- public transit scheduling optimization
- road maintenance budget allocation
- memoized route or resource optimization

### 4. Greedy Algorithms
Need:
- traffic signal optimization
- emergency vehicle priority handling
- real-time local decisions

### 5. Visualization / Demo Layer
Need:
- result visualization for graph algorithms
- sample scenarios for demo
- performance outputs for the report

---

## Development Notes

### Why the model relationships are hidden in JSON
The navigation properties are marked with `[JsonIgnore]` so API responses stay clean and do not recursively include related entities.

### Why migrations are used
You wanted the database to update when models change without manual database work. EF Core migrations provide the minimal version of that setup:
- change the model
- create a migration
- app applies it automatically on startup

### Why seeding is kept separate
The seed file is only for inserting the project data once when the database is empty.

### Suggested project direction
Keep the current structure and add algorithm services gradually. That will keep the codebase simple and easy to explain in the technical report.

---

## Run Guide

1. Open the solution in Visual Studio.
2. Make sure the `DefaultConnection` value in `appsettings.json` points to the SQLite file.
3. Run the app.
4. On startup, the app will:
   - migrate the database
   - seed the database if empty
   - open Swagger UI in development

### Useful URLs
- `http://localhost:5208/swagger`
- `https://localhost:7167/swagger`

---

## Technical Report Outline Suggestion

If you are writing the report now, use this structure:

1. Introduction
2. Problem Statement
3. System Architecture
4. Database Design
5. API Design
6. Implemented Algorithms
7. Complexity Analysis
8. Testing and Results
9. Challenges and Future Work
10. Conclusion

---

## Summary

This repository currently provides a solid foundation for the smart city transportation optimization project:
- clean entities
- API endpoints
- automatic schema updates
- one-time seeding
- Swagger documentation

The next step is to implement the algorithm modules required by the course brief.
