package com.softwave.transportsystem.graph.shared.model;

/**
 * Internal graph edge used by routing and spanning-tree algorithms.
 *
 * <p>
 * This model intentionally contains only graph data needed by the algorithms.
 * Human-readable names are kept in node lookups and response DTOs so the
 * business/API layer stays separate from the in-memory graph structure.
 * </p>
 */
public class GraphEdge {

    private final String fromId;
    private final String fromName;
    private final String toId;
    private final String toName;
    private final double distanceKm;

    public GraphEdge(String fromId, String toId, double distanceKm) {
        this(fromId, null, toId, null, distanceKm);
    }

    public GraphEdge(String fromId, String fromName,
            String toId, String toName,
            double distanceKm) {
        this.fromId = fromId;
        this.fromName = fromName;
        this.toId = toId;
        this.toName = toName;
        this.distanceKm = distanceKm;
    }

    public String getFromId() {
        return fromId;
    }

    public String getFromName() {
        return fromName;
    }

    public String getToId() {
        return toId;
    }

    public String getToName() {
        return toName;
    }

    public double getDistanceKm() {
        return distanceKm;
    }
}
