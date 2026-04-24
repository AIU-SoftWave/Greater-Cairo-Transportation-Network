# Graph Service

The **Graph Service** is the foundation for all algorithm implementations. It provides a basic graph structure that can be easily extended as new algorithms are implemented.

## Overview

The graph service provides:
- **Basic graph abstraction** - Nodes (locations) and edges (roads) with core metadata
- **Efficient lookups** - Adjacency lists and indexes for O(1) access
- **Incremental enhancement** - New methods and variants added as algorithms require them

## Architecture

```
Database (EF Core)
    ↓
EF Core queries for locations, roads, maintenance
    ↓
GraphService (assembles basic graph structure)
    ↓
Graph (nodes, edges, adjacency lists, indexes)
    ↓
Algorithm Services (extend as needed)
```

## Core Types

### GraphNode

Represents a location in the network:

```csharp
public class GraphNode
{
    public string Id { get; set; }                // Unique location ID
    public string Name { get; set; }              // Location name
    public string Type { get; set; }              // Type (Station, Hub, etc.)
    public double? X { get; set; }                // X coordinate
    public double? Y { get; set; }                // Y coordinate
    public int? Population { get; set; }          // Demand indicator
    public bool IsCritical { get; set; }          // Critical infrastructure flag
}
```

### GraphEdge

Represents a road in the network:

```csharp
public class GraphEdge
{
    public long Id { get; set; }                  // Road ID
    public string FromNodeId { get; set; }        // Source location
    public string ToNodeId { get; set; }          // Target location
    public double Distance { get; set; }          // Primary weight (distance in km)
    public int Capacity { get; set; }             // Vehicle capacity
    public int? Condition { get; set; }           // Road condition (1-5)
    public bool IsExisting { get; set; }          // Exists in network
    public double? ConstructionCost { get; set; } // Cost if planned
    public int? CurrentTraffic { get; set; }      // Traffic flow (optional)
    public int? MaintenancePriority { get; set; } // Maintenance urgency
    public double? MaintenanceCost { get; set; }  // Maintenance budget
}
```

### Graph

The complete graph structure:

```csharp
public class Graph
{
    public List<GraphNode> Nodes { get; set; }
    public List<GraphEdge> Edges { get; set; }
    public Dictionary<string, List<long>> AdjacencyList { get; set; }  // For O(1) neighbor lookup
    public Dictionary<string, GraphNode> NodeIndex { get; set; }       // For O(1) node lookup
    public Dictionary<long, GraphEdge> EdgeIndex { get; set; }         // For O(1) edge lookup
    public int NodeCount { get; }
    public int EdgeCount { get; }
}
```

## Current Interface

Currently, the graph service has one basic method:

### GetGraphAsync()

Gets the complete existing transportation network.

```csharp
Task<Graph> GetGraphAsync()
```

**Usage:**
- All algorithms start here
- Returns graph with all locations (nodes) and existing roads (edges)
- Includes maintenance metadata for each edge

**Returns:** Complete graph ready for algorithm processing

## How to Use

### Basic Graph Access

```csharp
var graph = await graphService.GetGraphAsync();

// Access all nodes
foreach (var node in graph.Nodes)
{
    Console.WriteLine($"{node.Id}: {node.Name}");
}

// Access all edges
foreach (var edge in graph.Edges)
{
    Console.WriteLine($"{edge.FromNodeId} → {edge.ToNodeId}: {edge.Distance} km");
}

// O(1) lookups
var node = graph.NodeIndex[nodeId];
var edge = graph.EdgeIndex[edgeId];

// Neighbors of a node
var neighborEdges = graph.AdjacencyList[nodeId];
foreach (var edgeId in neighborEdges)
{
    var edge = graph.EdgeIndex[edgeId];
    Console.WriteLine($"Neighbor: {edge.ToNodeId}");
}
```

## Design Principles

1. **Start Simple** - Begin with the most basic graph structure needed
2. **Extend Incrementally** - Add new methods as each algorithm requires them
3. **Efficient Access** - Maintain indexes and adjacency lists for O(1) lookups
4. **Rich Metadata** - Include all attributes algorithms might need (traffic, maintenance, condition)
5. **Immutable Snapshots** - Graphs are read-only after construction

## Planned Extensions

As new algorithms are implemented, the graph service will be extended with:

- `GetGraphWithTrafficAsync(period)` - When time-dependent routing is needed
- `GetGraphWithPlannedRoadsAsync()` - When MST expansion is implemented
- `GetCriticalSubgraphAsync()` - When critical infrastructure analysis is needed
- `GetSubgraphByBoundsAsync(...)` - When regional optimization is needed
- Edge query methods - When individual edge lookups are needed

**New methods will be added incrementally based on algorithm requirements.**

## Performance Characteristics

- **Graph Assembly** - O(n + m) where n = nodes, m = edges
- **Node Lookup** - O(1) via NodeIndex
- **Edge Lookup** - O(1) via EdgeIndex
- **Neighbor Traversal** - O(1) per edge via AdjacencyList
- **Memory** - O(n + m) for storing complete graph

## Next Steps

1. Build MST algorithm using GetGraphAsync()
2. Extend graph service with new methods as needed by subsequent algorithms
3. Consider caching for frequently accessed graphs
