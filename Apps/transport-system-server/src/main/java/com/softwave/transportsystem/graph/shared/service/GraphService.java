package com.softwave.transportsystem.graph.shared.service;

import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import com.softwave.transportsystem.graph.shared.model.GraphSnapshot;
import com.softwave.transportsystem.road.model.Road;
import com.softwave.transportsystem.road.repository.RoadRepository;
import com.softwave.transportsystem.shared.model.AbstractNode;
import com.softwave.transportsystem.shared.repository.NodeRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Collections;
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
 * {@link #buildGraphSnapshot()} therefore materialises <strong>both
 * directions</strong> of every road in the adjacency map so algorithms can
 * traverse each edge in either direction, while also exposing one canonical
 * edge per physical road for MST use.
 *
 * <h3>No caching</h3>
 * Methods build structures fresh from the database on each call. The dataset
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
     * @param nodeRepository provides all persisted nodes (neighborhoods +
     *                       facilities)
     */
    public GraphService(RoadRepository roadRepository, NodeRepository nodeRepository) {
        this.roadRepository = roadRepository;
        this.nodeRepository = nodeRepository;
    }

    /**
     * Builds a consistent in-memory snapshot of the road graph.
     *
     * <p>
     * The snapshot contains:
     * </p>
     * <ul>
     * <li>every known node name keyed by node ID, including isolated nodes</li>
     * <li>a bidirectional adjacency map for shortest-path algorithms</li>
     * <li>a canonical edge list with one edge per physical road for Kruskal's</li>
     * </ul>
     *
     * @return immutable-style graph snapshot for algorithm services
     */
    public GraphSnapshot buildGraphSnapshot() {
        Map<String, String> nodeNames = buildNodeNameMap();
        Map<String, List<GraphEdge>> adjacency = new LinkedHashMap<>();
        List<GraphEdge> edges = new ArrayList<>();

        for (String nodeId : nodeNames.keySet()) {
            adjacency.put(nodeId, new ArrayList<>());
        }

        for (Road road : roadRepository.findAll()) {
            String fromId = road.getFromNode().getNodeId();
            String toId = road.getToNode().getNodeId();
            double dist = road.getDistanceKm();

            adjacency.computeIfAbsent(fromId, k -> new ArrayList<>())
                    .add(new GraphEdge(fromId, toId, dist));

            adjacency.computeIfAbsent(toId, k -> new ArrayList<>())
                    .add(new GraphEdge(toId, fromId, dist));

            edges.add(new GraphEdge(fromId, toId, dist));
        }

        return new GraphSnapshot(
                Collections.unmodifiableMap(nodeNames),
                Collections.unmodifiableMap(adjacency),
                List.copyOf(edges));
    }

    /**
     * Backward-compatible access to the bidirectional adjacency map.
     *
     * @return node ID to outgoing edges
     */
    public Map<String, List<GraphEdge>> buildAdjacencyMap() {
        return buildGraphSnapshot().getAdjacency();
    }

    /**
     * Backward-compatible access to the canonical road edge list.
     *
     * @return one edge per persisted physical road
     */
    public List<GraphEdge> buildEdgeList() {
        return buildGraphSnapshot().getEdges();
    }

    /**
     * Returns a map from every known node ID to that node's human-readable name.
     *
     * <p>
     * The map covers all neighborhoods and facilities, including those not
     * yet connected to any road.
     * </p>
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

    /**
     * Returns all known nodes keyed by their node ID.
     *
     * @return node-ID to node entity mapping
     */
    public Map<String, AbstractNode> buildNodeMap() {
        Map<String, AbstractNode> nodeMap = new LinkedHashMap<>();
        for (AbstractNode node : nodeRepository.findAll()) {
            nodeMap.put(node.getNodeId(), node);
        }
        return nodeMap;
    }
}
