# Endpoints

## Locations
- `GET /api/locations`
- `GET /api/locations/{id}`

## Roads
- `GET /api/roads`
- `GET /api/roads/{id}`
- `GET /api/roads/from/{locationId}`
- `GET /api/roads/{roadId}/maintenance`

## Traffic
- `GET /api/traffic/road/{roadId}`
- `GET /api/traffic/period/{period}`

## Routes
- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/{id}/stops`

## Graph (Algorithm Foundation)
- `GET /api/graph` - Returns complete transportation network graph (nodes, edges, adjacency lists, indexes)

## Algorithms
- `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID` - Returns the shortest path using Dijkstra's algorithm and a rich DTO

## Response conventions
- `200 OK` when data is found
- `404 Not Found` when a single entity does not exist
- `400 Bad Request` when required query parameters are missing
- JSON output by default

## Notes
The endpoints are intentionally simple so they can later be reused by the algorithm modules.
The `/api/graph` endpoint provides the foundation for algorithm implementations by returning the complete network structure.
The shortest-path endpoint uses the basic graph service and Dijkstra's algorithm.