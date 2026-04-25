# Routing Module

## Purpose

Provides route search, emergency routing, and network expansion analysis.

## Controllers and Base Routes

- `AlgorithmsController` -> `GET /api/route-planning/*`
- `AStarController` -> `GET /api/emergency-routing`
- `MstController` -> `GET /api/network-expansion`
- `RoutesController` -> `GET /api/route-catalog/*`

## Endpoints

- `GET /api/route-planning/shortest-path?from=NODE_ID&to=NODE_ID`
- `GET /api/route-planning/time-route?from=NODE_ID&to=NODE_ID&period=MORNING`
- `GET /api/emergency-routing?from=NODE_ID&to=NODE_ID`
- `GET /api/network-expansion`
- `GET /api/route-catalog`
- `GET /api/route-catalog/{id}`
- `GET /api/route-catalog/{id}/stops`

## Services

- `RouteService`
- `DijkstraService`
- `TimeVaryingDijkstraService`
- `AStarService`
- `MstService`
