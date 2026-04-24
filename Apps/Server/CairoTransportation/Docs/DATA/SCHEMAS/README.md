# Database Schema

This page describes the SQLite tables used by the project.

## Tables
- `locations`
- `roads`
- `traffic_flow`
- `transport_routes`
- `route_stops`
- `transport_demand`
- `road_maintenance`

## How the schema is managed

The schema is controlled by Entity Framework Core migrations.
When the app starts:
1. EF Core checks the current database state.
2. EF Core applies any pending migrations.
3. If the database is empty, the seed script inserts the sample Cairo data.

## Relationship summary

### `roads`
- `from_location_id` → `locations.id`
- `to_location_id` → `locations.id`
- `road_maintenance.road_id` → `roads.id`
- `traffic_flow.road_id` → `roads.id`

### `route_stops`
- `route_id` → `transport_routes.id`
- `location_id` → `locations.id`

### `transport_demand`
- `from_location_id` → `locations.id`
- `to_location_id` → `locations.id`

## Schema design idea
The schema is intentionally simple and relational so it is easy to explain in class and easy to extend later.

## Recommended pages
- [Entity models](../MODELS/README.md)
- [EF Core and migrations](../EF-CORE.md)
- [Seeding](../SEEDING.md)