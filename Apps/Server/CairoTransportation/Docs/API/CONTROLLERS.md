# Controllers

Controllers are the HTTP entry points of the app.

## Current controllers
- `LocationsController` - Location data endpoints
- `RoadsController` - Road network endpoints
- `TrafficController` - Traffic flow endpoints
- `RoutesController` - Transit route endpoints
- `GraphController` (NEW) - Graph data for algorithms

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

This endpoint is the foundation for algorithm implementations.
It provides nodes, edges, adjacency lists, and indexes in a single call.

## Beginner summary
A controller is like the front desk of the API.
It gets the request and sends the work to the right service.

The graph controller is special: it returns the entire network structure needed by algorithms.