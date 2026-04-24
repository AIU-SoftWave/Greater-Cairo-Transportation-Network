# Controllers

Controllers are the HTTP entry points of the app.

## Current controllers
- `LocationsController` - browse and inspect network nodes such as neighborhoods and facilities
- `RoadsController` - browse roads, outgoing connections, and maintenance information
- `TrafficController` - inspect traffic flow by road or by time period
- `RoutesController` - inspect public transport routes and ordered stops
- `GraphController` - return the full graph structure used by algorithms
- `AlgorithmsController` - route search endpoints (Dijkstra currently implemented)

## What controllers should do

Controllers should:
- accept request parameters
- call services
- return HTTP responses

Controllers should not:
- contain database logic
- contain algorithm logic
- build SQL by hand

## GraphController

The `GraphController` exposes the graph service endpoint:
- `GET /api/graph` - Returns the complete transportation network graph

Use it when you want the full graph structure for algorithm development or debugging.

## AlgorithmsController

The `AlgorithmsController` currently exposes Dijkstra route search:
- `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID`
- `GET /api/algorithms/dijkstra/shortest-path?from=NODE_ID&to=NODE_ID`

Use it when you want the best general-purpose route by distance.

## Standard algorithm response shape

Algorithm endpoints should return the unified envelope:

```json
{
  "algorithmName": "Dijkstra",
  "success": true,
  "message": "Shortest path found using Dijkstra's algorithm.",
  "trace": {
    "visitedNodes": 87,
    "expandedNodes": 32,
    "executionTimeMs": 3
  },
  "data": {
    "fromNodeId": "1",
    "toNodeId": "13",
    "found": true,
    "totalDistance": 70.4,
    "pathNodes": [],
    "pathRoads": []
  }
}
```

### Trace semantics

- `visitedNodes`: count of unique discovered nodes.
- `expandedNodes`: count of nodes dequeued and processed.
- `executionTimeMs`: end-to-end service execution time.

### Status code behavior

- `200 OK`: `success = true`
- `404 Not Found`: valid request but no path / missing node
- `400 Bad Request`: invalid query parameters

## Guidance for adding next algorithm controller

1. Keep route under `/api/algorithms/...`.
2. Return `AlgorithmResponseDto<TData>` envelope.
3. Use shared trace metrics semantics.
4. Keep algorithm-specific details inside `data` + `message`.

## Beginner summary
A controller is like the front desk of the API.
It receives the request and sends the work to the correct service.