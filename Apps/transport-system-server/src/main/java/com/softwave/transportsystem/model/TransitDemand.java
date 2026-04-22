package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;

/**
 * Records observed public-transit passenger demand between two nodes.
 * Data source: transit_demand.csv
 *
 * Fields:
 * fromId – Origin node ID
 * toId – Destination node ID
 * dailyPassengers – Average daily passenger trips on this origin-destination
 * pair
 */
public class TransitDemand {

    private AbstractNode fromNode;
    private AbstractNode toNode;
    private int dailyPassengers;

    public TransitDemand() {
    }

    public TransitDemand(AbstractNode fromNode, AbstractNode toNode, int dailyPassengers) {
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.dailyPassengers = dailyPassengers;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public AbstractNode getFromNode() {
        return fromNode;
    }

    public void setFromNode(AbstractNode fromNode) {
        this.fromNode = fromNode;
    }

    public AbstractNode getToNode() {
        return toNode;
    }

    public void setToNode(AbstractNode toNode) {
        this.toNode = toNode;
    }

    public String getFromId() {
        return fromNode.getNodeId();
    }

    public String getToId() {
        return toNode.getNodeId();
    }

    public int getDailyPassengers() {
        return dailyPassengers;
    }

    public void setDailyPassengers(int p) {
        this.dailyPassengers = p;
    }

    public boolean involves(String nodeId) {
        return getFromId().equals(nodeId) || getToId().equals(nodeId);
    }
}
