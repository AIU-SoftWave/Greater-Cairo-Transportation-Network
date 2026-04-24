# Algorithms

This folder explains the algorithm side of the project.

## What this section is for

The project brief requires algorithmic solutions for:
- road network design
- route planning
- emergency routing
- time-varying traffic
- transit scheduling
- maintenance planning
- traffic control

These pages explain how the current data can support those algorithms.

## Pages
- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [**Graph Service** (NEW - Shared foundation for all algorithms)](GRAPH-SERVICE.md)
- [**Graph Service Quick Reference** (Developer guide with examples)](GRAPH-SERVICE-QUICK-REF.md)
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)
- [Implementation Roadmap](ROADMAP.md)
- [Diagrams](../DIAGRAMS/README.md)

---

## Implementation status

- ✅ Dijkstra shortest path is implemented.
- 🔄 A* shortest path is planned/refactoring target in this branch.
- ⏳ MST, time-dependent routing, DP, and greedy modules are pending.

---

## Shared algorithm contracts (refactored)

To prepare for multiple algorithms, shared contracts are now the default direction:

- `AlgorithmResponseDto<TData>`
  - `algorithmName`
  - `success`
  - `message`
  - `trace`
  - `data`

- `AlgorithmTraceDto`
  - `visitedNodes`
  - `expandedNodes`
  - `executionTimeMs`

- Shared shortest path data contracts:
  - `ShortestPathResultDto`
  - `ShortestPathNodeDto`
  - `ShortestPathRoadDto`

This prevents DTO duplication when adding MST, A*, time-dependent, DP, and greedy features.

---

## Centralized metrics collection

Algorithm services should use centralized instrumentation (`AlgorithmExecutionMetrics`) instead of inline stopwatch logic.

### Unified metric semantics

- `visitedNodes`: unique discovered nodes
- `expandedNodes`: nodes popped from queue and processed
- `executionTimeMs`: full algorithm service execution duration

### Edge-case conventions

- invalid input or missing node: `success = false`, counters typically remain zero
- same start and destination: `success = true`, zero distance, one discovered and expanded node
- no route: `success = false`, counters reflect work done until termination

---

## How the data behaves for algorithms

### Graph-based behavior
- `locations` are vertices
- `roads` are directed edges
- `distance`, `capacity`, and `condition` influence edge cost
- `isExisting` and `constructionCost` help compare built vs. possible roads

### Time-based behavior
- `traffic_flow` changes the cost of a road depending on period
- morning and evening traffic should increase weight
- night or off-peak periods should reduce weight

### Transit behavior
- `transport_routes` describe metro and bus networks
- `route_stops` describe ordered station/stop sequences
- `transport_demand` shows where riders are concentrated

### Maintenance behavior
- `road_maintenance` helps rank roads by priority and cost
- poor condition roads can be selected first by a planning algorithm

---

## Algorithm services and future endpoints

This section explains what each algorithm service should do and what endpoint it should expose when implemented.

### 1. MST / Road Network Design

#### Planned service
`MstService` or `GraphMstService`

#### What it should do
- build a low-cost network using locations and roads
- prefer existing roads when possible
- consider population and critical facilities as priority signals
- return selected roads, total cost, and connectivity explanation

#### Planned endpoint
- `GET /api/algorithms/mst`

#### Expected response
- selected road list
- total network cost
- connected locations
- notes about critical coverage

#### Data used
- `locations`
- `roads`

---

### 2. Shortest Path / Dijkstra

#### Current service
`DijkstraService`

#### Current endpoint
- `GET /api/algorithms/shortest-path?from=1&to=3`
- `GET /api/algorithms/dijkstra/shortest-path?from=1&to=3`

#### Current response shape
- wrapped inside `AlgorithmResponseDto<ShortestPathResultDto>`

#### Data used
- `roads`
- `locations`

---

### 3. Emergency Routing / A*

#### Planned service
`EmergencyRoutingService` or `AStarService`

#### What it should do
- find a fast path to a critical facility
- prefer routes that reduce estimated response time
- use a heuristic such as geographic distance
- account for road quality and traffic if needed

#### Planned endpoint
- `GET /api/algorithms/a-star?from=1&to=F9`

#### Expected response
- `AlgorithmResponseDto<ShortestPathResultDto>` envelope
- emergency path and explanation in `data/message`

#### Data used
- `locations`
- `roads`
- `traffic_flow`
- critical flags on locations

---

### 4. Time-Dependent Shortest Path

#### Planned service
`TimeAwareRouteService`

#### What it should do
- adjust edge costs by traffic period
- find routes that are best for morning, evening, or night
- show how congestion changes the answer

#### Planned endpoint
- `GET /api/algorithms/time-route?from=1&to=3&period=MORNING`

#### Expected response
- same standard algorithm envelope with time-adjusted route result payload

#### Data used
- `roads`
- `traffic_flow`

---

### 5. Dynamic Programming for Transit Scheduling

#### Planned service
`TransitSchedulingService`

#### What it should do
- allocate metro or bus service based on demand
- balance vehicle availability, passenger demand, and route coverage
- return an optimized schedule or allocation plan

#### Planned endpoint
- `GET /api/algorithms/transit-schedule`

---

### 6. Dynamic Programming for Maintenance Planning

#### Planned service
`MaintenancePlanningService`

#### Planned endpoint
- `GET /api/algorithms/maintenance-plan?budget=1000`

---

### 7. Greedy Traffic Signal Optimization

#### Planned service
`TrafficSignalService`

#### Planned endpoint
- `GET /api/algorithms/traffic-signals?period=MORNING`

---

### 8. Greedy Emergency Priority Handling

#### Planned service
`EmergencyPriorityService`

#### Planned endpoint
- `GET /api/algorithms/emergency-priority?from=1&to=F10`

---

## Refactoring rules before adding any new algorithm

1. Reuse shared DTO contracts instead of feature-specific duplicates.
2. Return standardized response envelope from service/controller.
3. Use centralized metrics instrumentation.
4. Keep controllers thin and consistent in status behavior.
5. Document algorithm endpoint + complexity notes in this folder.

---

## Beginner summary

The database is not just storage.
It is the input to the optimization algorithms.
The algorithms turn raw city data into decisions.

## More reading
- [Graph Data Behavior](GRAPH-DATA.md)
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)
- [Diagrams](../DIAGRAMS/README.md)