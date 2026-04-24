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

Use it when you want the best general-purpose route by distance.

## AStarController

The `AStarController` exposes A* route search:
- `GET /api/algorithms/a-star?from=NODE_ID&to=NODE_ID` - Returns a route using A* search

Use it when you want a coordinate-guided search, especially for emergency or target-focused routing.

## Unified trace metrics (Dijkstra + A*)

Both shortest-path endpoints return the same trace fields to support fair comparisons and easier onboarding for new algorithms:

- `visitedNodes`:
  - Count of unique nodes discovered by the algorithm.
  - A node is counted when its best-known cost is first assigned or improved.
  - Includes the start node when input is valid.

- `expandedNodes`:
  - Count of unique nodes expanded by the algorithm.
  - A node is expanded when dequeued from the priority queue and processed.

- `executionTimeMs`:
  - End-to-end execution time per request in milliseconds.
  - Includes graph fetch, search loop, and result mapping.

### Edge-case behavior (standardized)

- Invalid start or destination node:
  - `found = false`
  - `visitedNodes = 0`
  - `expandedNodes = 0`

- Same start and destination:
  - `found = true`
  - `totalDistance = 0`
  - `visitedNodes = 1`
  - `expandedNodes = 1`

- No route found:
  - `found = false`
  - counters reflect actual work done before termination

## Example response fields

```json
{
  "fromNodeId": "1",
  "toNodeId": "13",
  "found": true,
  "totalDistance": 70.4,
  "visitedNodes": 87,
  "expandedNodes": 32,
  "executionTimeMs": 3,
  "message": "Shortest path found using A* search.",
  "pathNodes": [],
  "pathRoads": []
}
```

## Guidance for adding the next algorithm

To keep results comparable, any new path algorithm should:

1. Return the same response contract fields.
2. Use the same metric semantics above.
3. Preserve the same edge-case behavior.
4. Keep algorithm-specific wording in `message` only.

This lets clients compare algorithms without changing parsing logic.

## Beginner summary
A controller is like the front desk of the API.
It receives the request and sends the work to the correct service.
The A* controller is especially useful when you need a route that searches toward a target more directly.