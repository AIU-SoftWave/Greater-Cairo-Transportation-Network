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
- **Philosophy**: Start simple, extend incrementally as algorithms require new graph variants

## Phase 3: Core Algorithms (NEXT) 🚀

### 3.1 MST / Road Network Design
- Implement Kruskal's or Prim's algorithm
- Use basic `GetGraphAsync()`
- *If needed for expansion analysis: extend graph service with `GetGraphWithPlannedRoadsAsync()`*

### 3.2 Dijkstra Shortest Path
- Implement single-source shortest path
- Use basic `GetGraphAsync()` with adjacency lists for efficient traversal
- *If needed: extend with traffic-aware variants*

### 3.3 A* Pathfinding
- Implement A* using node coordinates
- Use basic `GetGraphAsync()`
- *If needed: extend with custom heuristics*

### 3.4 Time-Dependent Routing
- Extend graph service with `GetGraphWithTrafficAsync(period)` when needed
- Incorporate traffic flow into path cost
- *Triggered only when implementing time-aware algorithms*

## Phase 4: Advanced Algorithms
- **Dynamic Programming** - transit scheduling
- **Greedy Methods** - maintenance prioritization
- New graph service methods added as needed

## Phase 5: Reporting and Demo
- Result DTOs for algorithm outputs
- Demo scenarios
- Performance measurements

---

## Graph Service Growth Plan

The graph service will evolve incrementally:

**Current (Phase 2):**
- ✅ `GetGraphAsync()` - basic graph with all nodes and existing roads

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
- Graph Service provides minimal but complete foundation
- Ready for first algorithm implementation (MST recommended)

**Next Steps:**
1. Build MST algorithm service → triggers Phase 3.1
2. Build Dijkstra service → triggers Phase 3.2
3. Evaluate if traffic features needed → conditionally triggers graph service extension
4. Continue with remaining algorithms

---

## Editing note
This roadmap reflects the incremental approach:
- Simple and complete in each phase
- Algorithms drive feature additions to graph service
- No speculative features added upfront