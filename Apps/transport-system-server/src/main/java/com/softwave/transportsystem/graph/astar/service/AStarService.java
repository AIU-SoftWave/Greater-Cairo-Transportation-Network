package com.softwave.transportsystem.graph.astar.service;

import com.softwave.transportsystem.graph.shared.dto.GraphNodeSummary;
import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import com.softwave.transportsystem.graph.shared.model.GraphSnapshot;
import com.softwave.transportsystem.graph.shared.service.GraphService;
import com.softwave.transportsystem.graph.shortestpath.dto.ShortestPathResult;
import com.softwave.transportsystem.shared.model.AbstractNode;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.PriorityQueue;
import java.util.Set;

/**
 * A* search for emergency routing.
 *
 * <h3>Algorithm description</h3>
 * A* extends Dijkstra's algorithm by adding a heuristic function
 * {@code h(n)} – typically the straight-line (Euclidean) distance from node
 * {@code n} to the destination using WGS-84 coordinates. The heuristic guides
 * the search towards the destination and dramatically reduces the number of
 * nodes expanded compared with plain Dijkstra on large graphs.
 *
 * <h3>Input data</h3>
 * <ul>
 * <li>{@code existing_roads.csv} – edges with {@code distance_km}</li>
 * <li>{@code nodes.csv} / {@code facilities.csv} – {@code latitude} and
 * {@code longitude} for the Euclidean heuristic</li>
 * </ul>
 *
 * <h3>Intended use case</h3>
 * Route an ambulance or emergency vehicle from its current location to the
 * nearest medical facility (F9 – Qasr El Aini, F10 – Ain Shams) in the
 * minimum possible travel time.
 *
 * <h3>Implementation owner</h3>
 * Assign to a team member following the contribution guide in
 * {@code PROJECT_OVERVIEW.md}.
 */
@Service
public class AStarService {

    private final GraphService graphService;

    public AStarService(GraphService graphService) {
        this.graphService = graphService;
    }

    /**
     * Finds the fastest emergency route between two nodes using A*.
     *
     * @param fromId source node ID (e.g. {@code "8"} for Giza)
     * @param toId   destination node ID (e.g. {@code "F9"} for hospital)
     * @return shortest path result based on road distance guided by a geographic
     *         heuristic
     */
    public ShortestPathResult findEmergencyPath(String fromId, String toId) {
        GraphSnapshot snapshot = graphService.buildGraphSnapshot();
        Map<String, String> nameMap = snapshot.getNodeNames();
        Map<String, AbstractNode> nodeMap = graphService.buildNodeMap();

        if (!nameMap.containsKey(fromId)) {
            return ShortestPathResult.notFound("Source node '" + fromId + "' was not found.");
        }
        if (!nameMap.containsKey(toId)) {
            return ShortestPathResult.notFound("Destination node '" + toId + "' was not found.");
        }
        if (fromId.equals(toId)) {
            return ShortestPathResult.found(
                    List.of(new GraphNodeSummary(fromId, nameMap.getOrDefault(fromId, fromId))),
                    0.0);
        }

        return runAStar(snapshot.getAdjacency(), nameMap, nodeMap, fromId, toId);
    }

    private ShortestPathResult runAStar(Map<String, List<GraphEdge>> adjacency,
            Map<String, String> nameMap,
            Map<String, AbstractNode> nodeMap,
            String fromId,
            String toId) {
        Map<String, Double> gScore = new HashMap<>();
        Map<String, String> predecessor = new HashMap<>();
        Set<String> closed = new HashSet<>();

        nameMap.keySet().forEach(id -> gScore.put(id, Double.MAX_VALUE));
        gScore.put(fromId, 0.0);

        PriorityQueue<NodeScore> openSet = new PriorityQueue<>();
        openSet.offer(new NodeScore(fromId, heuristicKm(nodeMap, fromId, toId)));

        while (!openSet.isEmpty()) {
            NodeScore next = openSet.poll();
            String current = next.nodeId();

            if (closed.contains(current)) {
                continue;
            }
            if (current.equals(toId)) {
                List<GraphNodeSummary> stops = reconstructPath(predecessor, nameMap, toId);
                return ShortestPathResult.found(stops, gScore.get(toId));
            }

            closed.add(current);

            for (GraphEdge edge : adjacency.getOrDefault(current, List.of())) {
                String neighbor = edge.getToId();
                double tentative = gScore.get(current) + edge.getDistanceKm();

                if (tentative < gScore.getOrDefault(neighbor, Double.MAX_VALUE)) {
                    gScore.put(neighbor, tentative);
                    predecessor.put(neighbor, current);
                    double estimatedTotal = tentative + heuristicKm(nodeMap, neighbor, toId);
                    openSet.offer(new NodeScore(neighbor, estimatedTotal));
                }
            }
        }

        return ShortestPathResult.notFound(
                "No path found between '" + fromId + "' and '" + toId + "'.");
    }

    private List<GraphNodeSummary> reconstructPath(Map<String, String> predecessor,
            Map<String, String> nameMap,
            String toId) {
        LinkedList<GraphNodeSummary> path = new LinkedList<>();
        String current = toId;

        while (current != null) {
            path.addFirst(new GraphNodeSummary(current, nameMap.getOrDefault(current, current)));
            current = predecessor.get(current);
        }

        return new ArrayList<>(path);
    }

    private double heuristicKm(Map<String, AbstractNode> nodeMap, String fromId, String toId) {
        AbstractNode from = nodeMap.get(fromId);
        AbstractNode to = nodeMap.get(toId);
        if (from == null || to == null) {
            return 0.0;
        }

        double lat1 = Math.toRadians(from.getLatitude());
        double lon1 = Math.toRadians(from.getLongitude());
        double lat2 = Math.toRadians(to.getLatitude());
        double lon2 = Math.toRadians(to.getLongitude());

        double deltaLat = lat2 - lat1;
        double deltaLon = lon2 - lon1;
        double a = Math.sin(deltaLat / 2) * Math.sin(deltaLat / 2)
                + Math.cos(lat1) * Math.cos(lat2)
                        * Math.sin(deltaLon / 2) * Math.sin(deltaLon / 2);
        double c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return 6371.0 * c;
    }

    private record NodeScore(String nodeId, double estimatedTotalCost)
            implements Comparable<NodeScore> {

        @Override
        public int compareTo(NodeScore other) {
            return Double.compare(this.estimatedTotalCost, other.estimatedTotalCost);
        }
    }
}
