package com.softwave.transportsystem.model.Interfaces;

/**
 * Shared base type for connections between two nodes.
 */
public abstract class AbstractEdge {

    private AbstractNode fromNode;
    private AbstractNode toNode;
    private double distanceKm;
    private int capacityVph;

    protected AbstractEdge() {}

    protected AbstractEdge(AbstractNode fromNode, AbstractNode toNode,
                           double distanceKm, int capacityVph) {
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.distanceKm = distanceKm;
        this.capacityVph = capacityVph;
    }

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



    public double getDistanceKm() {
        return distanceKm;
    }

    public void setDistanceKm(double distanceKm) {
        this.distanceKm = distanceKm;
    }

    public int getCapacityVph() {
        return capacityVph;
    }

    public void setCapacityVph(int capacityVph) {
        this.capacityVph = capacityVph;
    }


    private String getFromId() {
        return fromNode.getNodeId();
    }

    private String getToId() {
        return toNode.getNodeId();
    }


    public boolean connects(String nodeId) {
        return getFromId().equals(nodeId) || getToId().equals(nodeId);
    }

    public String asRoadId() {
        return getFromId() + "-" + getToId();
    }
}
