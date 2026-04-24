# Testing the Graph Endpoint

## Quick Test

Start the application and navigate to `/swagger` to access Swagger UI.

### Test the Graph Endpoint

1. **Find the Graph section** in Swagger UI
2. **Click** `GET /api/graph`
3. **Click** "Try it out"
4. **Click** "Execute"

### Expected Response

```json
{
  "nodes": [
    {
      "id": "L001",
      "name": "Downtown Station",
      "type": "Station",
      "x": -1.2945,
      "y": 30.0131,
      "population": 15000,
      "isCritical": true
    },
    // ... more nodes
  ],
  "edges": [
    {
      "id": 1,
      "fromNodeId": "L001",
      "toNodeId": "L002",
      "distance": 5.2,
      "capacity": 100,
      "condition": 2,
      "isExisting": true,
      "constructionCost": null,
      "currentTraffic": null,
      "maintenancePriority": 1,
      "maintenanceCost": 50000.0
    },
    // ... more edges
  ],
  "adjacencyList": {
    "L001": [1, 2, 3],
    "L002": [4, 5],
    // ... more nodes
  },
  "nodeIndex": {
    "L001": {...},
    "L002": {...},
    // ... all nodes by ID
  },
  "edgeIndex": {
    "1": {...},
    "2": {...},
    // ... all edges by ID
  },
  "trafficPeriod": null,
  "nodeCount": 12,
  "edgeCount": 25
}
```

## What Each Field Means

### Nodes Array
- `id` - Unique location identifier
- `name` - Location name
- `type` - Type of location (Station, Hub, Terminal, etc.)
- `x`, `y` - Geographic coordinates
- `population` - Demand/importance indicator
- `isCritical` - Whether this is critical infrastructure

### Edges Array
- `id` - Unique road identifier
- `fromNodeId` - Source location
- `toNodeId` - Target location
- `distance` - Road length in km (primary weight for algorithms)
- `capacity` - Vehicle capacity
- `condition` - Road condition (1-5 scale)
- `isExisting` - Whether the road exists (true) or is planned (false)
- `constructionCost` - Cost if road is planned
- `currentTraffic` - Current traffic flow (optional, null for basic graph)
- `maintenancePriority` - Maintenance urgency (lower = higher priority)
- `maintenanceCost` - Estimated maintenance cost

### AdjacencyList
- Maps node IDs to lists of edge IDs
- Used by algorithms for O(1) neighbor lookup
- Example: `"L001": [1, 2, 3]` means node L001 has outgoing edges 1, 2, and 3

### NodeIndex
- Maps node ID to full node object
- Enables O(1) node lookup by ID

### EdgeIndex
- Maps edge ID to full edge object
- Enables O(1) edge lookup by ID

### Meta Fields
- `trafficPeriod` - Which traffic period this graph represents (null for basic graph)
- `nodeCount` - Total number of nodes
- `edgeCount` - Total number of edges

## Using This Endpoint

### For Algorithm Development

When implementing an algorithm like MST or Dijkstra:

```csharp
var response = await httpClient.GetAsync("https://localhost:7123/api/graph");
var json = await response.Content.ReadAsStringAsync();
var graph = JsonSerializer.Deserialize<Graph>(json);

// Now you have the complete graph:
// - graph.Nodes - all locations
// - graph.Edges - all roads
// - graph.AdjacencyList - for neighbor traversal
// - graph.NodeIndex - for O(1) node lookup
// - graph.EdgeIndex - for O(1) edge lookup
```

### For Testing

This endpoint is useful for:
- Verifying graph structure loads correctly
- Checking that all nodes and edges are included
- Inspecting metadata (condition, maintenance priority, etc.)
- Performance testing (response time for full graph)

## Performance Notes

- **Response time**: Should be < 500ms on first call (builds graph from DB)
- **Response size**: Depends on number of nodes/edges (typically < 1MB)
- **Subsequent calls**: If caching is added, < 50ms

## Next Steps

Use this graph data endpoint when implementing the first algorithm (MST).
See: `Docs/ALGORITHMS/GRAPH-SERVICE-QUICK-REF.md` for usage patterns.
