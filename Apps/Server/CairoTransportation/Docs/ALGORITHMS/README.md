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
- ✅ A* shortest path is implemented.
- ✅ Time-Varying Dijkstra is implemented.
- ⏳ MST is pending.
- ⏳ Dynamic Programming modules are pending.
- ⏳ Greedy control modules are pending.

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

This prevents DTO duplication when adding MST, DP, and greedy features.

---

## Centralized metrics collection

Algorithm services should use centralized instrumentation (`AlgorithmExecutionMetrics`) instead of inline stopwatch logic.

### Unified metric semantics

- `visitedNodes`: unique discovered nodes
- `expandedNodes`: nodes popped from queue and processed
- `executionTimeMs`: full algorithm service execution duration

### Edge-case conventions

- invalid input or missing node: `success = false`
- same start and destination: `success = true`, zero distance
- no route: `success = false`, counters reflect work done until termination

---

## Model-to-algorithm implementation matrix

This matrix shows what is implemented based on the current backend models.

| Model / Data Source | Current usage | Status |
|---|---|---|
| `locations` | Node data for Dijkstra, A*, Time-Varying Dijkstra | ✅ Implemented |
| `roads` | Edge data for Dijkstra, A*, Time-Varying Dijkstra | ✅ Implemented |
| `traffic_flow` | Period traffic intensity in Time-Varying Dijkstra | ✅ Implemented |
| `traffic_period_multipliers` | DB-driven period validation and base multiplier selection | ✅ Implemented |
| `road_maintenance` | Loaded in graph metadata, not yet optimized | 🟡 Partial |
| `transport_routes` | Exposed via API, no optimization yet | ⏳ Not implemented |
| `route_stops` | Exposed via API, no optimization yet | ⏳ Not implemented |
| `transport_demand` | Exposed via API, no optimization yet | ⏳ Not implemented |

### Not implemented yet (algorithmic modules)

1. **MST / Infrastructure Optimization**
   - target models: `roads`, `locations`
   - planned endpoint: `GET /api/algorithms/mst`

2. **DP Maintenance Planning**
   - target models: `road_maintenance`, `roads.condition`
   - planned endpoint: `GET /api/algorithms/maintenance-plan?budget=...`

3. **DP Transit Scheduling**
   - target models: `transport_routes`, `route_stops`, `transport_demand`
   - planned endpoint: `GET /api/algorithms/transit-schedule`

4. **Greedy Traffic Signal Optimization**
   - target models: `traffic_flow`, `roads`
   - planned endpoint: `GET /api/algorithms/traffic-signals?period=...`

5. **Greedy Emergency Priority Handling**
   - target models: `locations` (critical flags), `roads`, `traffic_flow`
   - planned endpoint: `GET /api/algorithms/emergency-priority?from=...&to=...`

---

## How the data behaves for algorithms

### Graph-based behavior
- `locations` are vertices
- `roads` are directed edges
- `distance`, `capacity`, and `condition` influence edge cost
- `isExisting` and `constructionCost` help compare built vs. possible roads

### Time-based behavior
- `traffic_flow` changes congestion by period
- `traffic_period_multipliers` defines period-level base impact from DB

### Transit behavior
- `transport_routes` describe metro and bus networks
- `route_stops` describe ordered station/stop sequences
- `transport_demand` shows where riders are concentrated

### Maintenance behavior
- `road_maintenance` helps rank roads by priority and cost
- poor condition roads can be selected first by a planning algorithm

---

## Algorithm services and current endpoints

### 1. Shortest Path / Dijkstra

#### Current service
`DijkstraService`

#### Current endpoint
- `GET /api/algorithms/shortest-path?from=1&to=3`
- `GET /api/algorithms/dijkstra/shortest-path?from=1&to=3`

#### Response shape
- `AlgorithmResponseDto<ShortestPathResultDto>`

---

### 2. Emergency Routing / A*

#### Current service
`AStarService`

#### Current endpoint
- `GET /api/algorithms/a-star?from=1&to=F9`

#### Response shape
- `AlgorithmResponseDto<ShortestPathResultDto>`

---

### 3. Time-Varying Dijkstra

#### Current service
`TimeVaryingDijkstraService`

#### Current endpoint
- `GET /api/algorithms/time-route?from=1&to=3&period=MORNING`
- `GET /api/algorithms/time-varying-dijkstra/shortest-path?from=1&to=3&period=EVENING`

#### Response shape
- `AlgorithmResponseDto<ShortestPathResultDto>`

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