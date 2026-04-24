# Graph Service Quick Reference

## Quick Start

The graph service is intentionally minimal. It provides a basic graph structure that will grow incrementally as algorithms are added.

### Get the Graph

```csharp
public class MyAlgorithmService(IGraphService graphService)
{
    public async Task DoSomething()
    {
        var graph = await graphService.GetGraphAsync();
        
        // Work with nodes
        foreach (var node in graph.Nodes)
        {
            Console.WriteLine($"{node.Id}: {node.Name}");
        }
        
        // Work with edges
        foreach (var edge in graph.Edges)
        {
            Console.WriteLine($"{edge.FromNodeId} → {edge.ToNodeId}: {edge.Distance}");
        }
    }
}
```

## Common Patterns

### Pattern 1: Traverse from a Node

```csharp
var nodeId = "L001";
var edges = graph.AdjacencyList[nodeId];  // Get edge IDs connected to this node

foreach (var edgeId in edges)
{
    var edge = graph.EdgeIndex[edgeId];    // O(1) lookup
    var targetNode = graph.NodeIndex[edge.ToNodeId];
    Console.WriteLine($"Can go to: {targetNode.Name}");
}
```

### Pattern 2: Access Node Data

```csharp
// Direct access by ID (O(1))
var node = graph.NodeIndex["L001"];
Console.WriteLine($"Population: {node.Population}");
Console.WriteLine($"Critical: {node.IsCritical}");
```

### Pattern 3: Access Edge Data

```csharp
// Direct access by ID (O(1))
var edge = graph.EdgeIndex[42];
Console.WriteLine($"Distance: {edge.Distance}");
Console.WriteLine($"Condition: {edge.Condition}");
Console.WriteLine($"Maintenance Priority: {edge.MaintenancePriority}");
```

## Simple Dijkstra Example

```csharp
public async Task<List<string>> FindShortestPath(string from, string to)
{
    var graph = await graphService.GetGraphAsync();
    
    var distances = new Dictionary<string, double>();
    var previous = new Dictionary<string, string>();
    var unvisited = new HashSet<string>();

    // Initialize
    foreach (var node in graph.Nodes)
    {
        distances[node.Id] = double.MaxValue;
        unvisited.Add(node.Id);
    }
    distances[from] = 0;

    // Dijkstra
    while (unvisited.Count > 0)
    {
        var current = unvisited.OrderBy(n => distances[n]).First();
        if (current == to) break;
        unvisited.Remove(current);

        // Check neighbors using adjacency list
        if (graph.AdjacencyList.ContainsKey(current))
        {
            foreach (var edgeId in graph.AdjacencyList[current])
            {
                var edge = graph.EdgeIndex[edgeId];
                var neighbor = edge.ToNodeId;

                var newDistance = distances[current] + edge.Distance;
                if (newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                    previous[neighbor] = current;
                }
            }
        }
    }

    return ReconstructPath(previous, from, to);
}
```

## Simple MST Example (Kruskal's)

```csharp
public async Task<List<GraphEdge>> ComputeMST()
{
    var graph = await graphService.GetGraphAsync();
    
    // Sort edges by distance
    var edges = graph.Edges.OrderBy(e => e.Distance).ToList();
    var uf = new UnionFind(graph.NodeCount);
    var mst = new List<GraphEdge>();

    // Map node IDs to indices
    var nodeToIndex = graph.Nodes
        .Select((n, i) => (n.Id, i))
        .ToDictionary(x => x.Id, x => x.i);

    // Kruskal's algorithm
    foreach (var edge in edges)
    {
        var u = nodeToIndex[edge.FromNodeId];
        var v = nodeToIndex[edge.ToNodeId];

        if (uf.Find(u) != uf.Find(v))
        {
            mst.Add(edge);
            uf.Union(u, v);
        }
    }

    return mst;
}
```

## Graph Structure

```
graph.Nodes              -> List<GraphNode>        (all locations)
graph.Edges              -> List<GraphEdge>        (all roads)
graph.AdjacencyList      -> Dict<nodeId, List<edgeIds>>  (neighbors)
graph.NodeIndex          -> Dict<nodeId, GraphNode>      (O(1) lookup)
graph.EdgeIndex          -> Dict<edgeId, GraphEdge>      (O(1) lookup)
graph.NodeCount          -> int                    (total nodes)
graph.EdgeCount          -> int                    (total edges)
```

## Performance Notes

- **Node lookup**: `graph.NodeIndex[id]` is O(1)
- **Edge lookup**: `graph.EdgeIndex[id]` is O(1)
- **Neighbor traversal**: Loop through `graph.AdjacencyList[nodeId]` is O(degree)
- **All operations**: No database queries once graph is loaded

## What's NOT Included (Yet)

The following will be added as algorithms need them:
- Traffic data per period
- Planned/expansion roads
- Critical nodes subgraph
- Geographic bounding box queries

**Each will be added as an algorithm requires it.**

## See Also

- [Full Graph Service Documentation](GRAPH-SERVICE.md)
- [Algorithms Overview](OVERVIEW.md)
