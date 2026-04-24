# API Layer

This folder explains the REST API structure.

## What the API layer does

The API layer:
- receives HTTP requests
- routes them to controller actions
- returns JSON responses
- exposes Swagger in development

## Main API parts

- [Controllers](#controllers)
- [Endpoints](#endpoints)
- [Graph Endpoint (NEW)](#graph-endpoint-new)
- [Swagger and OpenAPI](#swagger-and-openapi)
- [Response shape](#response-shape)
- [Diagrams](../DIAGRAMS/README.md)

---

## Controllers

Controllers are the HTTP entry points of the app.
They are thin classes that call services and return responses.

Current controllers:
- `LocationsController` - browse and inspect nodes
- `RoadsController` - browse roads and maintenance data
- `TrafficController` - inspect traffic flow by road or period
- `RoutesController` - inspect public transport routes and stops
- `GraphController` - return the graph structure for algorithms
- `AlgorithmsController` - Dijkstra shortest path search
- `AStarController` - A* search for emergency and target-directed routing

### Controller responsibilities
- accept route parameters
- call the appropriate service
- return `200 OK`, `404 Not Found`, or other HTTP responses

### What controllers should not do
- database logic
- algorithm logic
- manual SQL

### Beginner explanation
A controller is like the front desk of the API.
It receives the request and sends the work to the correct service.

---

## Endpoints

### Locations
- `GET /api/locations`
- `GET /api/locations/{id}`

### Roads
- `GET /api/roads`
- `GET /api/roads/{id}`
- `GET /api/roads/from/{locationId}`
- `GET /api/roads/{roadId}/maintenance`

### Traffic
- `GET /api/traffic/road/{roadId}`
- `GET /api/traffic/period/{period}`

### Routes
- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/{id}/stops`

### Docs endpoints
- `GET /swagger`
- `GET /openapi/v1.json`

### Beginner explanation
Each endpoint is a URL that returns a piece of transportation data.

---

## Graph Endpoint (NEW)

The graph endpoint is special: it returns the entire transportation network structure needed by algorithms.

- **Endpoint**: `GET /api/graph`
- **Returns**: Complete graph with nodes, edges, adjacency lists, and indexes
- **Use**: Algorithms retrieve this once to get all network data

[Full documentation](GRAPH-ENDPOINT.md)

### Why separate from individual data endpoints?

Individual endpoints (`/api/locations`, `/api/roads`, etc.) return structured data for browsing.
The graph endpoint returns an algorithm-friendly structure optimized for computation.

---

## Swagger and OpenAPI

Swagger is the browser UI for exploring and testing the API.
OpenAPI is the machine-readable description behind that UI.

In this project:
- OpenAPI is enabled in `Program.cs`
- Swagger UI opens in development
- the browser launches to `/swagger`

Why it helps beginners:
- see all endpoints in one place
- test requests without writing client code
- understand request and response shapes

### Controller-specific documentation

Each controller also has a dedicated Swagger/OpenAPI description:
- **Locations**: view and search nodes
- **Roads**: browse roads and maintenance schedules
- **Traffic**: analyze traffic data by road or time period
- **Routes**: explore public transport routes and stops
- **Graph**: the transportation network structure
- **Algorithms**: Dijkstra and A* pathfinding algorithms

## Response shape

The API returns entity-shaped JSON responses.
That means responses include:
- scalar fields
- foreign key IDs

And exclude:
- navigation objects
- recursive nested entities

This keeps the API output clean and avoids circular JSON problems.

### Example
A road response includes:
- `fromLocationId`
- `toLocationId`

But not:
- `fromLocation`
- `toLocation`

This is handled with `[JsonIgnore]` on navigation properties.

### Why this matters for algorithms
The clean shape makes it easier for later algorithm endpoints to return results without nested object noise.

---

## Editing guidance

If you add new endpoints:
- keep controllers thin
- put logic in services
- document the endpoint here
- update Swagger if needed
