# EF Core and Migrations

## What EF Core does here

Entity Framework Core is the ORM used by the project.
It translates between:
- C# entity classes
- SQLite tables and rows

## Why EF Core is used

It keeps the project simple because you can work with normal C# objects instead of writing SQL for every operation.

## DbContext

`TransportationDbContext` is the main database context.
It contains the entity sets:
- `Locations`
- `Roads`
- `TrafficFlows`
- `TransportRoutes`
- `RouteStops`
- `TransportDemands`
- `RoadMaintenances`

It also configures the relationships between tables.

## Migrations

Migrations are the safe way to change the schema when a model changes.

### Workflow
1. Change a model.
2. Create a new migration.
3. Run the application.
4. The app applies the migration automatically.

## Why not `EnsureCreated`

`EnsureCreated` is simple but it does not support long-term schema evolution well.
Migrations are better for a project that will keep changing.

## Current startup behavior
The app runs:
- `Database.MigrateAsync()`
- then the seeder if the database is empty

## Beginner note
Think of EF Core as the bridge between the object world and the database world.