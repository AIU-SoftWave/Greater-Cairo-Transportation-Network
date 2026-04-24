# Controllers

Controllers are the HTTP entry points of the app.

## Current controllers
- `LocationsController` - Location data endpoints
- `RoadsController` - Road network endpoints
- `TrafficController` - Traffic flow endpoints
- `RoutesController` - Transit route endpoints
- `GraphController` - Graph data for algorithms
- `AlgorithmsController` - Algorithm endpoints such as shortest path

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

## AlgorithmsController

The `AlgorithmsController` exposes algorithm endpoints:
- `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID` - Returns the shortest path using Dijkstra's algorithm

This endpoint uses the graph service and a dedicated Dijkstra service.
It returns a rich DTO with path nodes, path roads, total distance, and status information.

## Beginner summary
A controller is like the front desk of the API.
It gets the request and sends the work to the right service.

The graph controller is special: it returns the entire network structure needed by algorithms.