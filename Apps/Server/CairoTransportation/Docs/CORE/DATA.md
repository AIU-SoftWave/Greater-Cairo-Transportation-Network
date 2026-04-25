# Data Layer

This page explains the data layer in the project.

## What the data layer does

The data layer is responsible for:
- storing transportation data
- mapping C# classes to database tables
- applying schema changes through migrations
- loading the initial Cairo dataset once

## What is inside the data layer

- [Models](#models)
- [Database schema](#database-schema)
- [EF Core and migrations](#ef-core-and-migrations)
- [Seeding](#seeding)

## Models

The model classes are the C# representation of the database tables.

### Model list
- `Location`
- `Road`
- `TrafficFlow`
- `TransportRoute`
- `RouteStop`
- `TransportDemand`
- `RoadMaintenance`

### Model behavior
- scalar properties become table columns
- foreign key properties store relationships by ID
- navigation properties connect related entities in code
- navigation properties are hidden from JSON responses using `[JsonIgnore]`

### Main purpose of each model
- **Location**: neighborhoods and facilities
- **Road**: road connections between locations, including whether they are one-way or two-way
- **TrafficFlow**: traffic volume by period
- **TransportRoute**: metro and bus routes
- **RouteStop**: ordered stops
- **TransportDemand**: origin-destination demand
- **RoadMaintenance**: maintenance priority and cost

## Database schema

The SQLite schema currently contains these tables:
- `locations`
- `roads`
- `traffic_flow`
- `transport_routes`
- `route_stops`
- `transport_demand`
- `road_maintenance`

### Relationships
- `roads.from_location_id` → `locations.id`
- `roads.to_location_id` → `locations.id`
- `traffic_flow.road_id` → `roads.id`
- `route_stops.route_id` → `transport_routes.id`
- `route_stops.location_id` → `locations.id`
- `transport_demand.from_location_id` → `locations.id`
- `transport_demand.to_location_id` → `locations.id`
- `road_maintenance.road_id` → `roads.id`

### Road direction behavior
- `is_two_way = true` means the road can be traveled in both directions
- `is_two_way = false` means the road is one-way from `from_location_id` to `to_location_id`
- the graph service adds reverse traversal edges for two-way roads so algorithms can route both directions

### Why the schema is simple
The schema is intentionally close to the project brief so it is easy to explain and easy to extend with new algorithm modules.

## EF Core and migrations

Entity Framework Core is the ORM used by the project.
It translates between C# objects and SQLite tables.

### DbContext
`TransportationDbContext` is the central database context.
It exposes the entity sets and configures relationships.

### Migration workflow
1. Change a model.
2. Add a migration.
3. Run the app.
4. The app automatically applies the migration.

### Why migrations are used
Migrations are the safe way to evolve the schema over time.
They are better than recreating the whole database every time the model changes.

## Seeding

The initial data lives in:
- `Data/TablesData.sql`

At startup the app checks whether the database is empty.
If it is empty, the seed SQL is executed once.

### Seeded data includes
- locations
- roads
- traffic flow samples
- transport routes
- route stops
- transport demand
- road maintenance samples

### Why seeding is separate
This keeps the startup process simple and makes the data easy to edit later.
