# Cairo Transportation Docs

Welcome to the documentation hub for the Cairo Transportation API.

This project is a beginner-friendly ASP.NET Core + EF Core application for the Greater Cairo transportation optimization project.

## Quick links

- [Start Here](START-HERE/README.md)
- [Modules](MODULES/README.md)
- [Core Docs](CORE/README.md)
- [Diagrams](DIAGRAMS/README.md)

## What this project currently includes

- ASP.NET Core API on .NET 10
- EF Core with SQLite
- automatic migrations on startup
- one-time seeding from `Data/TablesData.sql`
- Swagger/OpenAPI in development
- clean JSON entity responses
- business-first module routes (legacy aliases removed)

## What still needs to be built

- cross-module test coverage and validation suites
- tighter namespace/module alignment
- complete DTO + mapper consolidation into module folders
- visual demo outputs

## Folder map

- `MODULES/` – module-by-module documentation (source of truth)
- `CORE/` – shared framework, data, development, and project docs
- `START-HERE/` – quick beginner orientation
- `DIAGRAMS/` – PlantUML diagrams and PNG exports

## Editing strategy

Each major topic has one folder README so the docs are easy to maintain and extend later.