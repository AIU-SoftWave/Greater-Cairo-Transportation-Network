package com.softwave.transportsystem.graph.shared.dto;

/**
 * Simple API-friendly node summary carrying the identifier and display name.
 */
public class GraphNodeSummary {

    private final String id;
    private final String name;

    public GraphNodeSummary(String id, String name) {
        this.id = id;
        this.name = name;
    }

    public String getId() {
        return id;
    }

    public String getName() {
        return name;
    }
}
