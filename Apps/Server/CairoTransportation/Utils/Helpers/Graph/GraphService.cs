using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CairoTransportation.Services.Graph;

/// <summary>
/// Shared graph service implementation.
/// Assembles transportation network data from database and provides graph structures for algorithms.
/// The built graph is stored in an application-level in-memory cache (shared across all requests)
/// with a 30-second TTL. This means any request within the same 30-second window reuses the same
/// graph object and skips all database round-trips.
/// </summary>
public class GraphService(TransportationDbContext dbContext, IMemoryCache cache) : IGraphService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    public async Task<Graph> GetGraphAsync(bool includePotentialRoads = false)
    {
        string cacheKey = $"graph:{includePotentialRoads}";
        if (cache.TryGetValue(cacheKey, out Graph? cached) && cached is not null)
        {
            return cached;
        }


        var graph = new Graph();

        // Load all nodes (locations)
        List<Location> locations = await dbContext.Locations
            .AsNoTracking()
            .ToListAsync();

        foreach (Location? location in locations)
        {
            var node = new GraphNode
            {
                Id = location.Id,
                Name = location.Name,
                Type = location.Type,
                X = location.X,
                Y = location.Y,
                Population = location.Population,
                IsCritical = location.IsCritical
            };
            graph.Nodes.Add(node);
            graph.NodeIndex[node.Id] = node;
        }

        // Load edges
        IQueryable<Road> roadsQuery = dbContext.Roads.AsNoTracking();
        if (!includePotentialRoads)
        {
            roadsQuery = roadsQuery.Where(x => x.IsExisting);
        }

        List<Road> roads = await roadsQuery.ToListAsync();

        // Load maintenance data for edges
        Dictionary<long, RoadMaintenance> maintenanceMap = await dbContext.RoadMaintenances
            .AsNoTracking()
            .ToDictionaryAsync(x => x.RoadId);

        foreach (Road? road in roads)
        {
            AddEdge(graph, road, maintenanceMap, road.FromLocationId, road.ToLocationId, road.Id);

            if (road.IsTwoWay)
            {
                AddEdge(graph, road, maintenanceMap, road.ToLocationId, road.FromLocationId, -road.Id);
            }
        }

        cache.Set(cacheKey, graph, CacheTtl);

        return graph;
    }

    private static void AddEdge(
        Graph graph,
        Road road,
        Dictionary<long, RoadMaintenance> maintenanceMap,
        string fromNodeId,
        string toNodeId,
        long edgeId)
    {
        var edge = new GraphEdge
        {
            Id = edgeId,
            FromNodeId = fromNodeId,
            ToNodeId = toNodeId,
            Distance = road.Distance,
            Capacity = road.Capacity,
            Condition = road.Condition,
            IsExisting = road.IsExisting,
            ConstructionCost = road.ConstructionCost
        };

        if (maintenanceMap.TryGetValue(road.Id, out RoadMaintenance? maintenance))
        {
            edge.MaintenancePriority = maintenance.Priority;
            edge.MaintenanceCost = maintenance.EstimatedCost;
        }

        graph.Edges.Add(edge);
        graph.EdgeIndex[edge.Id] = edge;

        if (!graph.AdjacencyList.ContainsKey(edge.FromNodeId))
        {
            graph.AdjacencyList[edge.FromNodeId] = [];
        }

        graph.AdjacencyList[edge.FromNodeId].Add(edge.Id);
    }
}

