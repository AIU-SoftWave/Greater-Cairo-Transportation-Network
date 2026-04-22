package com.softwave.transportsystem.graph.service;

import com.softwave.transportsystem.graph.model.GraphEdge;
import com.softwave.transportsystem.road.model.Road;
import com.softwave.transportsystem.road.repository.RoadRepository;
import com.softwave.transportsystem.shared.model.AbstractNode;
import com.softwave.transportsystem.shared.repository.NodeRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/**
 * Builds and exposes an in-memory representation of the road network from the
 * persisted {@link Road} entities.
 *
 * <h3>Undirected treatment</h3>
 * Roads in {@code existing_roads.csv} are stored as directed rows (one row per
 * road segment), but the algorithm roadmap states they should be treated as
 * <em>undirected</em> for both MST and shortest-path computations.
 * {@link #buildAdjacencyMap()} therefore materialises <strong>both
 * directions</strong> of every road so that algorithms can traverse each edge
 * in either direction.  {@link #buildEdgeList()} returns one edge per physical
 * road (the canonical from→to direction) for use by Kruskal's algorithm.
 *
 * <h3>No caching</h3>
 * Methods build structures fresh from the database on each call.  The dataset
 * is small (≈ 28 roads, ≈ 19 nodes) so the overhead is negligible; avoiding a
 * server-level cache also means the graph always reflects the current DB state.
 */
@Service
public class GraphService {

    private final RoadRepository roadRepository;
    private final NodeRepository nodeRepository;

    /**
     * Constructs the service with its required repositories.
     *
     * @param roadRepository provides all persisted road edges
     * @param nodeRepository provides all persisted nodes (neighborhoods + facilities)
     */
    public GraphService(RoadRepository roadRepository, NodeRepository nodeRepository) {
        this.roadRepository = roadRepository;
        this.nodeRepository = nodeRepository;
    }

    // ------------------------------------------------------------------ public API

    /**
     * Builds a <em>bidirectional</em> adjacency map from all persisted roads.
     *
     * <p>Each physical road contributes two entries: one for the forward
     * direction (from→to) and one for the reverse direction (to→from).  This
     * lets Dijkstra's algorithm traverse the road network as an undirected
     * graph.</p>
     *
     * @return map from each node ID to the list of outgoing {@link GraphEdge}s
     */
    public Map<String, List<GraphEdge>> buildAdjacencyMap() {
        Map<String, String> nameMap = buildNodeNameMap();
        Map<String, List<GraphEdge>> adjacency = new LinkedHashMap<>();

        for (Road road : roadRepository.findAll()) {
            String fromId   = road.getFromNode().getNodeId();
            String toId     = road.getToNode().getNodeId();
            String fromName = nameMap.getOrDefault(fromId, fromId);
            String toName   = nameMap.getOrDefault(toId, toId);
            double dist     = road.getDistanceKm();

            // Forward edge (as stored in the database)
            adjacency.computeIfAbsent(fromId, k -> new ArrayList<>())
                     .add(new GraphEdge(fromId, fromName, toId, toName, dist));

            // Reverse edge (undirected treatment)
            adjacency.computeIfAbsent(toId, k -> new ArrayList<>())
                     .add(new GraphEdge(toId, toName, fromId, fromName, dist));
        }

        return adjacency;
    }

    /**
     * Collects the unique set of undirected edges from all persisted roads.
     *
     * <p>Each physical road produces exactly one {@link GraphEdge} entry (the
     * canonical from→to direction stored in the database).  This list is the
     * correct input for Kruskal's MST, which needs each edge exactly once.</p>
     *
     * @return list of unique edges suitable for Kruskal's algorithm
     */
    public List<GraphEdge> buildEdgeList() {
        Map<String, String> nameMap = buildNodeNameMap();
        List<GraphEdge> edges = new ArrayList<>();

        for (Road road : roadRepository.findAll()) {
            String fromId   = road.getFromNode().getNodeId();
            String toId     = road.getToNode().getNodeId();
            String fromName = nameMap.getOrDefault(fromId, fromId);
            String toName   = nameMap.getOrDefault(toId, toId);
            edges.add(new GraphEdge(fromId, fromName, toId, toName, road.getDistanceKm()));
        }

        return edges;
    }

    /**
     * Returns a map from every known node ID to that node's human-readable name.
     *
     * <p>The map covers all neighborhoods and facilities, including those not
     * yet connected to any road.</p>
     *
     * @return node-ID → display-name mapping
     */
    public Map<String, String> buildNodeNameMap() {
        Map<String, String> nameMap = new LinkedHashMap<>();
        for (AbstractNode node : nodeRepository.findAll()) {
            nameMap.put(node.getNodeId(), node.getName());
        }
        return nameMap;
    }
}
