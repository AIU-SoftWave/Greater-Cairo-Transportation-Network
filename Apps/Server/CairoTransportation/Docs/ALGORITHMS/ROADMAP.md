# Implementation Roadmap

## Phase 1: Data and API foundation ✅ DONE

- models
- DbContext
- migrations
- seeding
- controllers
- Swagger

## Phase 2: Graph foundation ✅ DONE

- Basic graph service (`IGraphService`, `GraphService`)
- Graph data structures (`GraphNode`, `GraphEdge`, `Graph`)
- Core functionality: `GetGraphAsync()` - loads all nodes and edges with adjacency lists and indexes
- Two-way road expansion for route traversal when `is_two_way = true`
- **Philosophy**: Start simple, extend incrementally as algorithms require new graph variants

## Phase 3: Core Algorithms 🚀

### 3.1 Dijkstra Shortest Path ✅ DONE

- Implemented as `IDijkstraService` / `DijkstraService`
- Endpoint: `GET /api/algorithms/shortest-path?from=NODE_ID&to=NODE_ID`
- Uses the basic graph and edge distance as the weight

### 3.2 A\* Pathfinding

- Implemented as `IAStarService` / `AStarService`
- Endpoint: `GET /api/algorithms/a-star?from=NODE_ID&to=NODE_ID`
- Uses the basic graph and coordinate heuristic

### 3.3 MST / Road Network Design

- Implemented Prim's algorithm as `MstService`
- Extended `GetGraphAsync(includePotentialRoads: true)` to include planned roads
- Endpoint: `GET /api/algorithms/mst`

### 3.4 Time-Dependent Routing

- Implemented as `TimeVaryingDijkstraService`
- Uses `ITrafficService` for period multipliers and traffic flow
- Endpoint: `GET /api/algorithms/time-route`

## Phase 4: Advanced Algorithms ✅ DONE

- **Dynamic Programming** - Maintenance planning (0/1 Knapsack) ✅ DONE
- **Dynamic Programming** - Transit scheduling (Resource Allocation DP) ✅ DONE
- **Greedy Methods** - Traffic signal optimization (planned)
- **Greedy Methods** - Emergency priority routing (planned)

## Phase 5: Reporting and Demo

- Result DTOs for algorithm outputs
- Demo scenarios
- Performance measurements

---

## Graph Service Growth Plan

The graph service will evolve incrementally:

**Current (Phase 2):**

- ✅ `GetGraphAsync()` - basic graph with all nodes and existing roads
- ✅ two-way road expansion for bidirectional travel

**Planned Additions (when algorithms require them):**

- `GetGraphWithTrafficAsync(period)` - when building time-dependent routing
- `GetGraphWithPlannedRoadsAsync()` - when building MST expansion analysis
- `GetCriticalSubgraphAsync()` - when building critical infrastructure analysis
- Geographic query methods - when building regional optimization
- Edge/node query methods - as individual lookups become needed

**Principle**: Don't add methods until an algorithm actually needs them.

---

## Current Status

**Just Completed:**

- Transit Scheduling Service with Resource Allocation DP
- Endpoint: `GET /api/algorithms/transit-schedule?vehicles=50`
- Allocates vehicles across routes to maximize demand coverage

**Implemented So Far:**

1. ✅ Dijkstra - shortest path by distance
2. ✅ A\* - heuristic-guided pathfinding
3. ✅ MST (Prim's) - cheapest network design
4. ✅ Time-Varying Dijkstra - traffic-aware routing
5. ✅ Maintenance Planning (0/1 Knapsack) - budget optimization
6. ✅ Transit Scheduling (Resource Allocation DP) - vehicle/fleet optimization

**Next Steps:**

1. Traffic Signal Service (Greedy) - real-time signal timing optimization
2. Emergency Priority Service (Greedy) - emergency vehicle priority routing

---

## Editing note

This roadmap reflects the incremental approach:

- Simple and complete in each phase
- Algorithms drive feature additions to graph service
- No speculative features added upfront- No speculative features added upfront
