# Start Here

This page gives a quick beginner-friendly tour of the project.

## What this project is

This is a REST API for a smart city transportation network.
It stores transportation data for Greater Cairo and prepares the foundation for route optimization and traffic algorithms.

## What the app currently does

- loads SQLite from configuration
- applies migrations on startup
- seeds the database once if it is empty
- exposes endpoints for locations, roads, traffic, and routes
- serves Swagger in development

## Main parts of the app

- `Program.cs` – app startup and dependency setup
- `Controllers/` – HTTP endpoints
- `Services/` – query logic
- `Models/` – database entities
- `Data/` – DbContext, seeding, migrations

## Reading order
1. [ASP.NET Core Basics](../ASP-NET-CORE/README.md)
2. [Data Layer](../DATA/README.md)
3. [API Layer](../API/README.md)
4. [Algorithms](../ALGORITHMS/README.md)
5. [Development Guide](../DEV/README.md)

## Beginner summary

If you are new to ASP.NET Core:
- a controller receives a request
- a service gets data or runs logic
- EF Core talks to the database
- the model classes define the tables

That is the basic flow in this project