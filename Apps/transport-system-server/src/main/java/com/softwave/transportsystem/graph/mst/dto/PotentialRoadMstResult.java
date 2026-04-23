package com.softwave.transportsystem.graph.mst.dto;

import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import java.util.List;

/**
 * Result of Prim's MST over the proposed road network.
 */
public class PotentialRoadMstResult {

    private final List<GraphEdge> edges;
    private final int totalConstructionCostMEgp;
    private final double totalDistanceKm;
    private final int nodeCount;
    private final int edgeCount;
    private final boolean connected;
    private final String message;

    public PotentialRoadMstResult(List<GraphEdge> edges,
            int totalConstructionCostMEgp,
            double totalDistanceKm,
            int nodeCount,
            int edgeCount,
            boolean connected,
            String message) {
        this.edges = edges;
        this.totalConstructionCostMEgp = totalConstructionCostMEgp;
        this.totalDistanceKm = totalDistanceKm;
        this.nodeCount = nodeCount;
        this.edgeCount = edgeCount;
        this.connected = connected;
        this.message = message;
    }

    public List<GraphEdge> getEdges() {
        return edges;
    }

    public int getTotalConstructionCostMEgp() {
        return totalConstructionCostMEgp;
    }

    public double getTotalDistanceKm() {
        return totalDistanceKm;
    }

    public int getNodeCount() {
        return nodeCount;
    }

    public int getEdgeCount() {
        return edgeCount;
    }

    public boolean isConnected() {
        return connected;
    }

    public String getMessage() {
        return message;
    }
}
