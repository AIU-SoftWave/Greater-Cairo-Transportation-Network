# Cairo Transportation Docs

Welcome to the documentation hub for the Cairo Transportation API.

This project is a beginner-friendly ASP.NET Core + EF Core application for the Greater Cairo transportation optimization project.

## Quick links

- [Start Here](START-HERE/README.md)
- [ASP.NET Core Basics](ASP-NET-CORE/README.md)
- [Data Layer](DATA/README.md)
- [API Layer](API/README.md)
- [Algorithms](ALGORITHMS/README.md)
- [Development Guide](DEV/README.md)
- [Project Goals](PROJECT/README.md)
- [Diagrams](DIAGRAMS/README.md)

## What this project currently includes

- ASP.NET Core API on .NET 10
- EF Core with SQLite
- automatic migrations on startup
- one-time seeding from `Data/TablesData.sql`
- Swagger/OpenAPI in development
- clean JSON entity responses

## What still needs to be built

- MST / road network optimization
- shortest path algorithms
- emergency routing
- time-dependent routing
- dynamic programming optimization
- greedy traffic control
- visual demo outputs

## Folder map

- `START-HERE/` – quick beginner orientation
- `ASP-NET-CORE/` – how the web framework works here
- `DATA/` – models, schema, EF Core, and seeding
- `API/` – controllers, endpoints, Swagger, and response shape
- `ALGORITHMS/` – algorithm goals, behavior, and roadmap
- `DEV/` – setup, libraries, and contribution notes
- `PROJECT/` – status, goals, and remaining work
- `DIAGRAMS/` – PlantUML diagrams and PNG exports

## Editing strategy

Each major topic has one folder README so the docs are easy to maintain and extend later.