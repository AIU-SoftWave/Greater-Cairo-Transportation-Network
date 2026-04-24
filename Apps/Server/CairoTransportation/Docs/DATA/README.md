# Data Layer

This folder explains the data layer in the project.

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
- [Diagrams](../DIAGRAMS/README.md)

---

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
- **Road**: directed road connections
- **TrafficFlow**: traffic volume by period
- **TransportRoute**: metro and bus routes
- **RouteStop**: ordered route stops
- **TransportDemand**: origin-destination demand
- **RoadMaintenance**: maintenance priority and cost

### How models relate to algorithms
- `Location` and `Road` form the graph used by MST and shortest path algorithms
- `TrafficFlow` changes edge cost over time
- `TransportRoute` and `RouteStop` support transit scheduling
- `TransportDemand` helps measure load and allocation pressure
- `RoadMaintenance` supports repair prioritization

---

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

### Why the schema is simple
The schema is intentionally close to the project brief so it is easy to explain and easy to extend with new algorithm modules.

### Schema behavior for algorithms
- `locations` are graph vertices
- `roads` are directed weighted edges
- `traffic_flow` adds time-based weight changes
- `route_stops` and `transport_routes` describe public transit paths
- `transport_demand` measures OD pressure
- `road_maintenance` supports road repair prioritization

---

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

### Beginner explanation
Think of EF Core as the bridge between C# objects and database tables.
The models describe the shape of the data, and the DbContext tells EF Core how everything connects.

---

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

### Beginner explanation
The seeder is just startup code that says:
- if there is no data, insert the starting dataset
- if data already exists, do nothing

---

## Editing guidance

If you change anything in the data layer:
- update the model
- update the DbContext mapping if needed
- create a migration
- update seed data if the sample dataset changes
- run the app and check Swagger for the result