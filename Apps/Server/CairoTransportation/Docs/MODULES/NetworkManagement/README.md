# Network Management Module

## Purpose

Exposes transportation network nodes, roads, and graph topology used by algorithms.

## Controllers and Base Routes

- `LocationsController` -> `GET /api/city-locations/*`
- `RoadsController` -> `GET /api/road-network/*`
- `GraphController` -> `GET /api/network-topology`

## Endpoints

- `GET /api/city-locations`
- `GET /api/city-locations/{id}`
- `GET /api/road-network`
- `GET /api/road-network/{id}`
- `GET /api/road-network/from/{locationId}`
- `GET /api/road-network/{roadId}/maintenance`
- `GET /api/network-topology`

## Services

- `LocationService`
- `RoadService`
- `GraphService`
