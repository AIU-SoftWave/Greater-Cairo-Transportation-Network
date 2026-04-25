using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.Graph;

/// <summary>
/// Shared graph service implementation.
/// Assembles transportation network data from database and provides graph structures for algorithms.
/// This will be extended incrementally as new algorithms require additional graph variants.
/// </summary>
public class GraphService(TransportationDbContext dbContext) : IGraphService
{
    public async Task<Graph> GetGraphAsync(bool includePotentialRoads = false)
    {
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

