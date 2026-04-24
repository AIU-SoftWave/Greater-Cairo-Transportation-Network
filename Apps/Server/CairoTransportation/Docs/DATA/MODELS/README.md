# Models

This page explains the entity classes used by EF Core.

## Model list
- `Location`
- `Road`
- `TrafficFlow`
- `TransportRoute`
- `RouteStop`
- `TransportDemand`
- `RoadMaintenance`

## What a model is

A model is a C# class that represents one database table.
The properties on the class map to columns in the table.

## How models behave in this project

- scalar properties become table columns
- foreign key properties store relationships by ID
- navigation properties connect related entities in code
- navigation properties are hidden from JSON responses using `[JsonIgnore]`

## Why this matters

Keeping the models close to the database makes the project easier to understand for beginners.
It also makes it easier to generate migrations when the schema changes.

## Entity summary

### Location
Represents both neighborhoods and facilities.

### Road
Represents a directed road between two locations.

### TrafficFlow
Represents traffic data for a road and a period.

### TransportRoute
Represents a metro or bus route.

### RouteStop
Represents the ordered stops of a route.

### TransportDemand
Represents passenger demand between two locations.

### RoadMaintenance
Represents maintenance data for a road.

## Editing rule

If you change a model:
- update the DbContext mapping if needed
- create a migration
- run the app so the database updates automatically