# Minimum Spanning Tree (MST)

## Overview

The MST algorithm builds the **cheapest possible road network** that connects all locations (cities, neighborhoods, facilities) in the transportation system. It uses **Prim's algorithm** to guarantee the minimum total construction cost while ensuring every location is reachable.

## Real-World Purpose

- **Network Design**: Plan new road infrastructure at minimum cost
- **Budget Optimization**: Decide which potential roads to build
- **Expansion Planning**: Connect isolated facilities to the existing network
- **Resource Allocation**: Prioritize construction projects by cost-effectiveness

## How It Works

### Prim's Algorithm (Greedy Approach)

Think of growing a tree branch by branch:

1. **Start** at any node (e.g., "Maadi")
2. **Examine** all roads leaving the current network
3. **Pick** the cheapest road that reaches a new, unconnected location
4. **Add** that location to the network
5. **Repeat** until all locations are connected

This greedy strategy guarantees the **minimum total cost** — mathematically proven optimal for MST.

### Cost Function: Existing vs Potential Roads

```csharp
if (road.IsExisting)
    return 0;  // Already built — FREE!
else
    return road.ConstructionCost;  // Must pay to build
```

| Road Type                              | Cost                 | Why                              |
| -------------------------------------- | -------------------- | -------------------------------- |
| **Existing** (`IsExisting = true`)     | **0**                | Already built, use for free      |
| **Potential** (`IsExisting = false`)   | **ConstructionCost** | Must budget for new construction |
| **Invalid** (no cost for non-existing) | **∞**                | Ignored — can't build there      |

### Why This Approach Works

1. **Existing roads are always preferred first** — they're free
2. **Potential roads only chosen when necessary** — to reach isolated nodes
3. **Minimum cost potential road selected** — among all options to reach a new node
4. **Optimal result guaranteed** — no cheaper network exists for the same connectivity

## API Endpoint

### `GET /api/algorithms/mst`

Builds the cheapest spanning tree connecting all locations.

#### Response Structure

```json
{
  "algorithmName": "MST",
  "success": true,
  "message": "Cheapest network built using MST (Prim's algorithm).",
  "trace": {
    "visitedNodes": 35,
    "expandedNodes": 34,
    "executionTimeMs": 85
  },
  "data": {
    "connected": true,
    "totalConstructionCost": 28500000,
    "totalNodes": 35,
    "selectedRoadCount": 34,
    "nodes": [...],
    "selectedRoads": [...]
  }
}
```

#### Response Fields

| Field                   | Description                                                                 |
| ----------------------- | --------------------------------------------------------------------------- |
| `connected`             | `true` if all nodes are reachable, `false` if graph is disconnected         |
| `totalConstructionCost` | Sum of all `ConstructionCost` for selected potential roads                  |
| `totalNodes`            | Total locations in the graph                                                |
| `selectedRoadCount`     | Number of roads in the spanning tree (always `totalNodes - 1` if connected) |
| `nodes`                 | All locations in the network                                                |
| `selectedRoads`         | Chosen roads (mix of existing and potential)                                |

## Implementation Details

### Service Architecture

```
MstController
    ↓
IMstService (MstService)
    ↓
IGraphService (GraphService with includePotentialRoads: true)
    ↓
Database (Locations + Roads)
```

### Key Design Decisions

1. **Graph includes potential roads** — `GetGraphAsync(includePotentialRoads: true)` loads both existing and planned roads
2. **Undirected edges** — Two-way roads are normalized to avoid duplicates
3. **Priority queue frontier** — Efficiently finds minimum cost edge at each step
4. **Greedy expansion** — Always adds cheapest connection to unvisited node

### Algorithm Complexity

- **Time**: O(E log V) — where E is edges, V is vertices (nodes)
- **Space**: O(V + E) — for adjacency lists and priority queue

## Example Walkthrough

Given your Cairo transportation network:

### Initial State

- 35 locations (nodes)
- 30 connected via existing roads
- 5 isolated facilities (F3, F4, F5, F6, F10) with no roads

### After Adding Potential Roads

```sql
-- 5 potential roads added to connect isolated facilities
F3 → Giza (8M)         -- Cairo University
F4 → Downtown (6M)     -- Al-Azhar University
F5 → Downtown (4M)     -- Egyptian Museum
F6 → Nasr City (5M)    -- Stadium
F10 → Maadi (5.5M)     -- Maadi Military Hospital
```

### MST Result

- **29 existing roads** selected (cost: 0)
- **5 potential roads** selected (cost: 28.5M)
- **Total**: 34 roads connecting 35 nodes for **28,500,000**

This is **mathematically optimal** — no cheaper connected network exists.

## When Graph Is Disconnected

If some nodes have **no roads at all** (existing or potential), the algorithm returns:

```json
{
  "success": false,
  "message": "Graph is disconnected; a spanning tree covering all cities could not be built.",
  "data": {
    "connected": false,
    "totalNodes": 35,
    "selectedRoadCount": 30 // Only connected component
  }
}
```

**Solution**: Add potential roads to connect isolated nodes, then re-run.

## Extending the Algorithm

### Possible Enhancements

1. **Weighted Priority for Critical Facilities**

   ```csharp
   cost = baseCost / (isCritical ? 2.0 : 1.0);  // Prefer critical nodes
   ```

2. **Population-Weighted Cost**

   ```csharp
   cost = baseCost / Math.Log(population + 1);  // Prioritize high-population areas
   ```

3. **Multi-Objective Optimization**
   - Minimize cost AND maximize capacity
   - Minimize cost AND minimize distance

4. **Budget-Constrained MST**
   - Given a maximum budget, find best partial network
   - Return "what's the best we can build with X million?"

## Files

| File                                               | Purpose                              |
| -------------------------------------------------- | ------------------------------------ |
| `Services/Algorithms/Mst/MstService.cs`            | Core Prim's algorithm implementation |
| `Services/Algorithms/Mst/Contracts/IMstService.cs` | Service interface                    |
| `Services/Algorithms/Mst/DTOs/MstResultDto.cs`     | Response data structure              |
| `Controllers/MstController.cs`                     | API endpoint                         |

## Related Pages

- [Overview](OVERVIEW.md) — Algorithm system architecture
- [Graph Data Behavior](GRAPH-DATA.md) — How graph structures work
- [Graph Service](GRAPH-SERVICE.md) — IGraphService details
- [Shortest Path](SHORTEST-PATH.md) — Dijkstra/A\* for routing
- [Diagrams](../DIAGRAMS/README.md) — Visual architecture
