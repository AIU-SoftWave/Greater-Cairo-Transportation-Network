# Graph Endpoint Quick Reference

## Endpoint

```
GET /api/graph
```

## Response Structure

```
Graph
├── Nodes[]              // All locations
├── Edges[]              // All existing roads
├── AdjacencyList{}      // Node ID → List of edge IDs
├── NodeIndex{}          // Node ID → Node object
├── EdgeIndex{}          // Edge ID → Edge object
├── TrafficPeriod        // null for basic graph
├── NodeCount            // int
└── EdgeCount            // int
```

## Using in C#

```csharp
// Inject graph service
public class MyService(IGraphService graphService)
{
    public async Task Compute()
    {
        // Get graph
        var graph = await graphService.GetGraphAsync();

        // Access nodes
        var node = graph.NodeIndex["L001"];

        // Access edges
        var edge = graph.EdgeIndex[42];

        // Get neighbors
        var neighborEdges = graph.AdjacencyList["L001"];
    }
}
```

## Using in HTTP Client

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://localhost:7123/api/graph");
var json = await response.Content.ReadAsStringAsync();
var graph = JsonSerializer.Deserialize<Graph>(json);
```

## Node Fields

```
GraphNode
├── Id                   // "L001"
├── Name                 // "Downtown Station"
├── Type                 // "Station"
├── X                    // -1.2945
├── Y                    // 30.0131
├── Population           // 15000
└── IsCritical           // true
```

## Edge Fields

```
GraphEdge
├── Id                   // 1
├── FromNodeId           // "L001"
├── ToNodeId             // "L002"
├── Distance             // 5.2 (km)
├── Capacity             // 100
├── Condition            // 2 (1-5 scale)
├── IsExisting           // true
├── ConstructionCost     // null or double
├── CurrentTraffic       // null for basic graph
├── MaintenancePriority  // 1 (lower = higher priority)
└── MaintenanceCost      // 50000.0
```

## Performance

- First call: ~500ms (builds from DB)
- Subsequent calls: Depends on caching (< 50ms with caching)
- Response size: Typically < 1MB

## Testing

**In Swagger UI:**
1. Start app
2. Go to `/swagger`
3. Find Graph section
4. Try the GET endpoint
5. Execute

## Next

Use this endpoint when implementing algorithms.
See: `Docs/ALGORITHMS/GRAPH-SERVICE-QUICK-REF.md`
