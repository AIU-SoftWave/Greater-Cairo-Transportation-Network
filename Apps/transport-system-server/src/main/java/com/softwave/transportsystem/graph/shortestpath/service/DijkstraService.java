package com.softwave.transportsystem.graph.shortestpath.service;

import com.softwave.transportsystem.graph.shared.dto.GraphNodeSummary;
import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import com.softwave.transportsystem.graph.shared.model.GraphSnapshot;
import com.softwave.transportsystem.graph.shared.service.GraphService;
import com.softwave.transportsystem.graph.shortestpath.dto.ShortestPathResult;
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
 * Implements Dijkstra's single-source shortest-path algorithm on the existing
 * road network, using {@code distance_km} as the edge weight.
 *
 * <h3>Algorithm summary</h3>
 * <ol>
 * <li>Initialise every node's tentative distance to {@code ∞} except the
 * source, which starts at {@code 0}.</li>
 * <li>Use a min-heap (priority queue) keyed on tentative distance to always
 * relax the nearest un-visited node first.</li>
 * <li>When the destination node is dequeued, the shortest path has been
 * found. The actual route is reconstructed by following the predecessor
 * map backwards from the destination to the source and reversing the
 * list.</li>
 * <li>If the priority queue empties before reaching the destination, the two
 * nodes are in different connected components and no path exists.</li>
 * </ol>
 *
 * <h3>Lazy-deletion optimisation</h3>
 * Rather than updating existing heap entries when a shorter path is found, a
 * new entry is pushed with the improved distance and stale entries are
 * discarded when dequeued using a {@code visited} set. This gives
 * O((V + E) log V) time complexity.
 *
 * <h3>Edge weight</h3>
 * {@code distance_km} from {@code existing_roads.csv} is used as the edge
 * weight, consistent with the project specification: "Standard route planning
 * between any two nodes."
 *
 * <h3>Undirected graph</h3>
 * Roads are treated as undirected; {@link GraphService#buildGraphSnapshot()}
 * materialises both directions of every road before this service is called.
 */
@Service
public class DijkstraService {

    private final GraphService graphService;

    /**
     * Constructs the service with its graph-building dependency.
     *
     * @param graphService builds the in-memory adjacency structure from the DB
     */
    public DijkstraService(GraphService graphService) {
        this.graphService = graphService;
    }

    // public API

    /**
     * Finds the shortest road-distance path between {@code fromId} and
     * {@code toId} using Dijkstra's algorithm.
     *
     * <p>
     * Node IDs follow the same convention as the CSV data: numeric strings
     * for neighborhoods (e.g. {@code "1"}, {@code "13"}) and facility-prefixed
     * strings for facilities (e.g. {@code "F1"}, {@code "F9"}).
     * </p>
     *
     * @param fromId the starting node ID
     * @param toId   the destination node ID
     * @return a {@link ShortestPathResult} with the ordered path stops and
     *         total distance on success, or a descriptive failure result when
     *         either node is unknown or no path connects them
     */
    public ShortestPathResult findShortestPath(String fromId, String toId) {
        GraphSnapshot snapshot = graphService.buildGraphSnapshot();
        Map<String, List<GraphEdge>> adjacency = snapshot.getAdjacency();
        Map<String, String> nameMap = snapshot.getNodeNames();

        // Validate against all known nodes, not only connected nodes.
        if (!nameMap.containsKey(fromId)) {
            return ShortestPathResult.notFound(
                    "Source node '" + fromId + "' was not found.");
        }
        if (!nameMap.containsKey(toId)) {
            return ShortestPathResult.notFound(
                    "Destination node '" + toId + "' was not found.");
        }

        if (fromId.equals(toId)) {
            String name = nameMap.getOrDefault(fromId, fromId);
            return ShortestPathResult.found(List.of(new GraphNodeSummary(fromId, name)), 0.0);
        }

        return runDijkstra(adjacency, nameMap, fromId, toId);
    }

    // internals

    /**
     * Core Dijkstra loop. Assumes both endpoints exist in the node-name map.
     *
     * @param adjacency bidirectional adjacency map (node ID → outgoing edges)
     * @param nameMap   node ID → display name lookup
     * @param fromId    source node ID
     * @param toId      destination node ID
     * @return populated {@link ShortestPathResult}
     */
    private ShortestPathResult runDijkstra(Map<String, List<GraphEdge>> adjacency,
            Map<String, String> nameMap,
            String fromId, String toId) {
        Map<String, Double> dist = new HashMap<>();
        Map<String, String> predecessor = new HashMap<>();
        Set<String> visited = new HashSet<>();

        nameMap.keySet().forEach(id -> dist.put(id, Double.MAX_VALUE));
        dist.put(fromId, 0.0);

        PriorityQueue<Map.Entry<Double, String>> pq = new PriorityQueue<>(
                Map.Entry.comparingByKey());
        pq.offer(Map.entry(0.0, fromId));

        while (!pq.isEmpty()) {
            Map.Entry<Double, String> entry = pq.poll();
            double currDist = entry.getKey();
            String current = entry.getValue();

            if (visited.contains(current)) {
                continue;
            }
            if (current.equals(toId)) {
                break;
            }
            visited.add(current);

            for (GraphEdge edge : adjacency.getOrDefault(current, List.of())) {
                String neighbor = edge.getToId();
                double newDist = currDist + edge.getDistanceKm();

                if (newDist < dist.getOrDefault(neighbor, Double.MAX_VALUE)) {
                    dist.put(neighbor, newDist);
                    predecessor.put(neighbor, current);
                    pq.offer(Map.entry(newDist, neighbor));
                }
            }
        }

        double totalDist = dist.getOrDefault(toId, Double.MAX_VALUE);
        if (totalDist == Double.MAX_VALUE) {
            return ShortestPathResult.notFound(
                    "No path found between '" + fromId + "' and '" + toId + "'.");
        }

        List<GraphNodeSummary> stops = reconstructPath(predecessor, nameMap, toId);
        return ShortestPathResult.found(stops, totalDist);
    }

    /**
     * Walks the predecessor map from {@code toId} back to {@code fromId} and
     * returns the stops in forward (source-to-destination) order.
     *
     * @param predecessor map from each visited node to the node it was reached from
     * @param nameMap     node ID → display name
     * @param toId        destination node ID (starting point of back-trace)
     * @return ordered list of node summaries from source to destination
     */
    private List<GraphNodeSummary> reconstructPath(Map<String, String> predecessor,
            Map<String, String> nameMap,
            String toId) {
        LinkedList<GraphNodeSummary> path = new LinkedList<>();
        String current = toId;

        while (current != null) {
            String name = nameMap.getOrDefault(current, current);
            path.addFirst(new GraphNodeSummary(current, name));
            current = predecessor.get(current);
        }

        return new ArrayList<>(path);
    }
}
