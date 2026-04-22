package com.softwave.transportsystem.graph.model;

/**
 * Lightweight, immutable value object representing a single directed hop in
 * the in-memory road graph.
 *
 * <p>When {@link com.softwave.transportsystem.graph.service.GraphService}
 * builds the adjacency structure from {@code existing_roads.csv}, every
 * physical road is materialised as <em>two</em> {@code GraphEdge} instances –
 * one per direction – so that Dijkstra and Kruskal can traverse the network
 * in both directions (undirected treatment).</p>
 *
 * <p>For Kruskal's MST the edge list returned by
 * {@link com.softwave.transportsystem.graph.service.GraphService#buildEdgeList()}
 * contains exactly one {@code GraphEdge} per physical road (the canonical
 * from→to direction stored in the database).</p>
 */
public class GraphEdge {

    /** Node ID of the tail of this directed edge (e.g. {@code "1"} or {@code "F2"}). */
    private final String fromId;

    /** Human-readable name of the tail node. */
    private final String fromName;

    /** Node ID of the head of this directed edge. */
    private final String toId;

    /** Human-readable name of the head node. */
    private final String toName;

    /** Road length in kilometres – used as the edge weight for both Dijkstra and MST. */
    private final double distanceKm;

    /**
     * Constructs a fully initialised, immutable graph edge.
     *
     * @param fromId     tail node identifier
     * @param fromName   human-readable tail node name
     * @param toId       head node identifier
     * @param toName     human-readable head node name
     * @param distanceKm road length in kilometres
     */
    public GraphEdge(String fromId, String fromName,
                     String toId,   String toName,
                     double distanceKm) {
        this.fromId     = fromId;
        this.fromName   = fromName;
        this.toId       = toId;
        this.toName     = toName;
        this.distanceKm = distanceKm;
    }

    // ------------------------------------------------------------------ getters

    public String getFromId()     { return fromId; }
    public String getFromName()   { return fromName; }
    public String getToId()       { return toId; }
    public String getToName()     { return toName; }
    public double getDistanceKm() { return distanceKm; }
}
