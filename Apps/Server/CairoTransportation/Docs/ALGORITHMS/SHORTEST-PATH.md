# Shortest Path

## Purpose

Shortest path algorithms help the system find the best route between two locations.

## Data used
- `roads` for graph edges
- `distance` for base cost
- `traffic_flow` for time-based cost
- `roads.isExisting` and `condition` for route quality

## Dijkstra

Dijkstra is a standard shortest path algorithm.
Use it when:
- you want the normal best route
- weights are non-negative
- traffic is not strongly dynamic

### Behavior in this project
It can treat each road as an edge with a cost based on distance and possibly other factors like condition.

## A*

A* is useful for emergency routing.
Use it when:
- you want a faster path search
- you have a heuristic such as geographic distance to the goal
- the destination is a medical or critical facility

### Behavior in this project
A* can prioritize roads that move the route closer to the target while still respecting traffic and road quality.

## Time-dependent shortest path

This version should change edge weights depending on the period.
Examples:
- morning rush hour
- evening rush hour
- night traffic

### Behavior in this project
A road can become more expensive when traffic volume is high.
That means the best route can change depending on the time of day.

## Service design

### Planned services
- `DijkstraService`
- `AStarService`
- `TimeAwareRouteService`

### What each service should do
- **DijkstraService**: find the normal shortest route between two nodes
- **AStarService**: estimate and search for emergency-friendly routes using a heuristic
- **TimeAwareRouteService**: adjust weights using traffic period and calculate the best route for that time

### What the services should return
- path as a list of locations or roads
- total cost or distance
- travel-time estimate
- explanation for emergency or traffic-aware decisions

## Planned endpoints
- `GET /api/algorithms/shortest-path?from=1&to=3`
- `GET /api/algorithms/emergency-route?from=1&to=F9`
- `GET /api/algorithms/time-route?from=1&to=3&period=MORNING`

## Expected outputs
- path as a list of locations or roads
- total cost or distance
- travel-time estimate
- explanation for emergency or traffic-aware decisions

## Beginner summary
Shortest path is about finding the cheapest or fastest route, but in this project the cost can change with traffic.

## Related pages
- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [Diagrams](../DIAGRAMS/README.md)