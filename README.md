# Greater Cairo Transportation Network

A REST API for the Greater Cairo metropolitan transportation network, built with Spring Boot.
The API exposes all transportation data (neighborhoods, facilities, roads, traffic patterns,
metro lines, bus routes, OD demand) and hosts algorithmic modules for route planning,
network optimisation, and resource allocation.

## Quick Start

```bash
cd Apps/transport-system-server
mvn spring-boot:run
# Server starts at http://localhost:8080
```

## Documentation

See **[PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)** for:
- Full architecture description
- Data schema reference
- Complete API reference with all endpoints
- **Algorithm implementation checklist** (what is done vs. what needs to be implemented)
- **Step-by-step guide** for students adding their own algorithm module
- PlantUML diagram descriptions

## Implemented Algorithms

| Algorithm | Endpoint |
|---|---|
| Dijkstra's Shortest Path | `GET /api/graph/shortest-path?from={id}&to={id}` |
| Kruskal's MST | `GET /api/graph/mst` |

## Algorithm Placeholders (to be implemented)

| Algorithm | Endpoint |
|---|---|
| A* Emergency Routing | `GET /api/graph/astar?from={id}&to={id}` |
| Time-Varying Dijkstra | `GET /api/graph/time-varying-shortest-path?from={id}&to={id}&timeSlot={slot}` |
| Prim's MST | `GET /api/graph/prim-mst` |
| Greedy Signal Timing | `GET /api/traffic/signal-timing?timeSlot={slot}` |
| DP Road Maintenance | `GET /api/roads/maintenance-plan?budget={millions}` |
| DP Bus Fleet Scheduling | `GET /api/bus/fleet-optimisation` |
| DP Metro Frequency | `GET /api/metro/frequency-optimisation` |

Placeholder endpoints return `"Not implemented: <Algorithm Name>"` until implemented.
See Section 7 of `PROJECT_OVERVIEW.md` for the contribution guide.
