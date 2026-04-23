package com.softwave.transportsystem.graph.shared.model;

import java.util.List;
import java.util.Map;

/**
 * Immutable snapshot of the current road graph built from persisted nodes and
 * roads.
 */
public class GraphSnapshot {

    private final Map<String, String> nodeNames;
    private final Map<String, List<GraphEdge>> adjacency;
    private final List<GraphEdge> edges;

    public GraphSnapshot(Map<String, String> nodeNames,
            Map<String, List<GraphEdge>> adjacency,
            List<GraphEdge> edges) {
        this.nodeNames = nodeNames;
        this.adjacency = adjacency;
        this.edges = edges;
    }

    public Map<String, String> getNodeNames() {
        return nodeNames;
    }

    public Map<String, List<GraphEdge>> getAdjacency() {
        return adjacency;
    }

    public List<GraphEdge> getEdges() {
        return edges;
    }
}
