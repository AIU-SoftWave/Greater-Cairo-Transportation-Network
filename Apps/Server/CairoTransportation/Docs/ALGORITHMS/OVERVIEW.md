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
- **A\*** - coordinate-guided shortest path, useful for emergency and target-directed routing
- **MST** - cheapest network design connecting all locations
- **Time-Varying Dijkstra** - traffic-aware routing with period-based multipliers
- **Maintenance Planning (DP)** - optimal road repair selection using 0/1 Knapsack

## When to use them

| Algorithm                 | Use When                                                   |
| ------------------------- | ---------------------------------------------------------- |
| **Dijkstra**              | Normal route planning, best path by distance               |
| **A\***                   | Emergency routing, faster target search, map-guided routes |
| **MST**                   | Network expansion planning, cheapest connectivity          |
| **Time-Varying Dijkstra** | Rush hour routing, traffic-aware pathfinding               |
| **Maintenance Planning**  | Budget-constrained road repair optimization                |

## API endpoints

| Endpoint                                              | Algorithm             | Purpose                        |
| ----------------------------------------------------- | --------------------- | ------------------------------ |
| `GET /api/algorithms/shortest-path?from=X&to=Y`       | Dijkstra              | Shortest path by distance      |
| `GET /api/algorithms/a-star?from=X&to=Y`              | A\*                   | Heuristic-guided shortest path |
| `GET /api/algorithms/mst`                             | MST (Prim's)          | Cheapest network design        |
| `GET /api/algorithms/time-route?from=X&to=Y&period=Z` | Time-Varying Dijkstra | Traffic-aware routing          |
| `GET /api/algorithms/maintenance-plan?budget=X`       | 0/1 Knapsack DP       | Optimal repair selection       |
