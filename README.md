# Cairo Transportation Network

A .NET 10 REST API for the Greater Cairo transportation optimization project.

## Current Status

Implemented foundation:
- ASP.NET Core API
- Entity Framework Core
- SQLite database
- automatic migration application on startup
- one-time seeding from `Data/TablesData.sql`
- OpenAPI/Swagger UI
- core transportation entities and endpoints

## Main Endpoints

- `GET /api/locations`
- `GET /api/locations/{id}`
- `GET /api/roads`
- `GET /api/roads/{id}`
- `GET /api/roads/from/{locationId}`
- `GET /api/roads/{roadId}/maintenance`
- `GET /api/traffic/road/{roadId}`
- `GET /api/traffic/period/{period}`
- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/{id}/stops`

## Docs

See `PROJECT_OVERVIEW.md` for:
- architecture
- repository layout
- data model
- API reference
- what is implemented
- what still needs to be implemented for the course brief

## Run

Open the solution in Visual Studio and run the app.

Swagger opens in development at:
- `/swagger`

## Project Goal

The long-term goal is to implement the algorithmic modules required by the course brief:
- MST / road network design
- Dijkstra / A* / time-dependent routing
- dynamic programming for transit and maintenance
- greedy traffic signal and emergency prioritization
- reporting and visualization support


-hi there

## Seed Data

The sample Cairo dataset is stored in:
- `Apps/Server/CairoTransportation/Data/TablesData.sql`

It is inserted automatically only when the database is empty.
