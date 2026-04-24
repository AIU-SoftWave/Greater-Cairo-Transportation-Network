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
    public async Task<Graph> GetGraphAsync()
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

        // Load all edges (existing roads only)
        List<Road> roads = await dbContext.Roads
            .AsNoTracking()
            .Where(x => x.IsExisting)
            .ToListAsync();

        // Load maintenance data for edges
        Dictionary<long, RoadMaintenance> maintenanceMap = await dbContext.RoadMaintenances
            .AsNoTracking()
            .ToDictionaryAsync(x => x.RoadId);

        foreach (Road? road in roads)
        {
            var edge = new GraphEdge
            {
                Id = road.Id,
                FromNodeId = road.FromLocationId,
                ToNodeId = road.ToLocationId,
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

            // Build adjacency list
            if (!graph.AdjacencyList.ContainsKey(edge.FromNodeId))
            {
                graph.AdjacencyList[edge.FromNodeId] = [];
            }

            graph.AdjacencyList[edge.FromNodeId].Add(edge.Id);
        }

        return graph;
    }
}
