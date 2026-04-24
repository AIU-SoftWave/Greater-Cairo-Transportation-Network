# Controllers

Controllers are the HTTP entry points of the app.

## Current controllers
- `LocationsController` - browse and inspect network nodes such as neighborhoods and facilities
- `RoadsController` - browse roads, outgoing connections, and maintenance information
- `TrafficController` - inspect traffic flow by road or by time period
- `RoutesController` - inspect public transport routes and ordered stops
- `GraphController` - return the full graph structure used by algorithms
- `AlgorithmsController` - route search endpoints such as Dijkstra shortest path
- `AStarController` - coordinate-guided route search for emergency and fast target-directed routing

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

The `AlgorithmsController` exposes algorithm endpoints:
- `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID` - Returns the shortest path using Dijkstra's algorithm
- `GET /api/algorithms/dijkstra/shortest-path?from=NODE_ID&to=NODE_ID` - Alias for Dijkstra shortest path

Use it when you want the best general-purpose route by distance.

## AStarController

The `AStarController` exposes A* route search:
- `GET /api/algorithms/a-star?from=NODE_ID&to=NODE_ID` - Returns a route using A* search

Use it when you want a coordinate-guided search, especially for emergency or target-focused routing.

## Standard algorithm response shape

Algorithm endpoints should move to the unified envelope:

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

## Unified trace metrics

- `visitedNodes`: unique discovered nodes.
- `expandedNodes`: nodes dequeued and processed.
- `executionTimeMs`: end-to-end execution time in milliseconds.

### Edge-case behavior

- Invalid start or destination: failed result with zero counters.
- Same start and destination: success with zero distance.
- No route found: failed result with counters reflecting actual work.

## Guidance for adding the next algorithm

1. Keep route under `/api/algorithms/...`.
2. Return standardized response shape.
3. Use shared trace metric semantics.
4. Keep algorithm-specific detail in `message` + `data`.

## Beginner summary
A controller is like the front desk of the API.
It receives the request and sends the work to the correct service.
The A* controller is especially useful when you need a route that searches toward a target more directly.
