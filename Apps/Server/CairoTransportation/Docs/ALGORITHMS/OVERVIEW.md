# Algorithm Overview

## Goal
Use Cairo transportation data to build algorithmic solutions for a smart city network.

## Main data the algorithms work with
- `locations` → graph vertices
- `roads` → graph edges
- `traffic_flow` → time-based edge weights
- `transport_routes` and `route_stops` → transit network structure
- `transport_demand` → demand-based optimization
- `road_maintenance` → maintenance prioritization

## How the algorithms should behave

### Graph algorithms
They treat the network as a weighted directed graph.

### Time-aware algorithms
They should adjust road weights using traffic volume and time period.

### Planning algorithms
They should choose combinations of roads, routes, or maintenance actions that optimize cost or benefit.

## Important idea
The database is not just storage.
It is the input to the optimization algorithms.

## Detailed algorithm pages
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)

## Diagrams
- [Diagrams folder](../DIAGRAMS/README.md)

## Current implemented algorithms
- **Dijkstra** - best general-purpose shortest path by distance
- **A*** - coordinate-guided shortest path, useful for emergency and target-directed routing

## When to use them
- **Dijkstra**: use for normal route planning when you want the best path by distance
- **A***: use for emergency routing, faster target search, or map-guided route finding

## API endpoints
- `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID`
- `GET /api/algorithms/a-star?from=NODE_ID&to=NODE_ID`