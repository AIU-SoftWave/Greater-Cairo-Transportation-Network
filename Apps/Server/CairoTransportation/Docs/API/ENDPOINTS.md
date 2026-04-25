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
- `GET /api/traffic/period-multipliers`
- `GET /api/traffic/period-multipliers/{period}`

## Routes
- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/{id}/stops`

## Graph (Algorithm Foundation)
- `GET /api/graph` - Returns complete transportation network graph (nodes, edges, adjacency lists, indexes)

## Algorithms
- `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID` - Returns shortest path using Dijkstra
- `GET /api/algorithms/dijkstra/shortest-path?from=NODE_ID&to=NODE_ID` - Alias for Dijkstra endpoint
- `GET /api/algorithms/a-star?from=NODE_ID&to=NODE_ID` - Returns shortest path using A*
- `GET /api/algorithms/time-route?from=NODE_ID&to=NODE_ID&period=MORNING` - Returns traffic-aware shortest path using Time-Varying Dijkstra
- `GET /api/algorithms/time-varying-dijkstra/shortest-path?from=NODE_ID&to=NODE_ID&period=EVENING` - Alias for Time-Varying Dijkstra endpoint

## Algorithm response conventions
Algorithms should return the standardized envelope:

```json
{
  "algorithmName": "Time-Varying Dijkstra",
  "success": true,
  "message": "Traffic-aware shortest path found for period 'MORNING' using time-varying Dijkstra.",
  "trace": {
    "visitedNodes": 12,
    "expandedNodes": 9,
    "executionTimeMs": 2
  },
  "data": {
    "fromNodeId": "1",
    "toNodeId": "13",
    "found": true,
    "totalDistance": 78.5,
    "pathNodes": [],
    "pathRoads": []
  }
}
```

## HTTP status conventions
- `200 OK` when algorithm execution succeeds
- `404 Not Found` when request is valid but no path can be found / nodes missing
- `400 Bad Request` when required query parameters are missing

## Notes
The endpoints are intentionally simple so they can be reused by algorithm modules.
`/api/graph` provides the shared graph structure consumed by routing services.