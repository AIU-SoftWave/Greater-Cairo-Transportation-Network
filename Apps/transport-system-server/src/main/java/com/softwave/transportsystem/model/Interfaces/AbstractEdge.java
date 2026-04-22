package com.softwave.transportsystem.model.Interfaces;

import jakarta.persistence.Column;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.MappedSuperclass;

/**
 * Shared base type for connections between two nodes.
 */
@MappedSuperclass
public abstract class AbstractEdge {

    @ManyToOne(fetch = FetchType.EAGER, optional = false)
    @JoinColumn(name = "from_node_id", nullable = false)
    private AbstractNode fromNode;

    @ManyToOne(fetch = FetchType.EAGER, optional = false)
    @JoinColumn(name = "to_node_id", nullable = false)
    private AbstractNode toNode;

    @Column(nullable = false)
    private double distanceKm;

    @Column(nullable = false)
    private int capacityVph;

    protected AbstractEdge() {
    }

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

    protected String fromId() {
        return fromNode.getNodeId();
    }

    protected String toId() {
        return toNode.getNodeId();
    }

    public boolean connects(String nodeId) {
        return fromId().equalsIgnoreCase(nodeId) || toId().equalsIgnoreCase(nodeId);
    }

    public String asRoadId() {
        return fromId() + "-" + toId();
    }
}
