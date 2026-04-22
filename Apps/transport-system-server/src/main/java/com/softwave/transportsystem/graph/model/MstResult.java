package com.softwave.transportsystem.graph.model;

import java.util.List;

/**
 * Encapsulates the result of Kruskal's Minimum Spanning Tree algorithm run on
 * the existing road network.
 *
 * <h3>Semantics</h3>
 * The MST connects every node that participates in at least one road in
 * {@code existing_roads.csv} using the <em>minimum total road distance</em>
 * ({@code distance_km}) as the edge weight.  Because roads are treated as
 * undirected, the result is a spanning tree of the undirected road graph.
 *
 * <p>For a connected graph with <em>N</em> nodes, the MST contains exactly
 * <em>N&minus;1</em> edges, which is reflected by
 * {@link #getEdgeCount()} == {@link #getNodeCount()} - 1.</p>
 *
 * <h3>Usage</h3>
 * The MST reveals the minimum total road-length backbone that keeps every
 * district and facility connected – useful for understanding the
 * minimum-infrastructure requirement of the current network.
 */
public class MstResult {

    /**
     * The N-1 edges that form the Minimum Spanning Tree, in the order Kruskal's
     * algorithm selected them (ascending distance_km).
     */
    private final List<GraphEdge> edges;

    /** Sum of {@code distanceKm} for every edge in the MST. */
    private final double totalDistanceKm;

    /** Number of unique nodes (graph vertices) present in the road network. */
    private final int nodeCount;

    /** Number of edges selected for the MST (should equal {@code nodeCount - 1}). */
    private final int edgeCount;

    /**
     * Constructs a fully populated MST result.
     *
     * @param edges           the spanning-tree edges chosen by Kruskal's algorithm
     * @param totalDistanceKm cumulative road distance of all MST edges
     * @param nodeCount       number of unique nodes in the road network
     * @param edgeCount       number of edges in the MST
     */
    public MstResult(List<GraphEdge> edges, double totalDistanceKm,
                     int nodeCount,         int edgeCount) {
        this.edges           = edges;
        this.totalDistanceKm = totalDistanceKm;
        this.nodeCount       = nodeCount;
        this.edgeCount       = edgeCount;
    }

    // ------------------------------------------------------------------ getters

    public List<GraphEdge> getEdges()          { return edges; }
    public double          getTotalDistanceKm(){ return totalDistanceKm; }
    public int             getNodeCount()      { return nodeCount; }
    public int             getEdgeCount()      { return edgeCount; }
}
