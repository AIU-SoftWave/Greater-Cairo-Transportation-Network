package com.softwave.transportsystem.model.Interfaces;

import com.fasterxml.jackson.annotation.JsonIgnore;

/**
 * Shared base type for graph endpoints such as neighborhoods and facilities.
 * Stores the common location and classification fields used across the model.
 */
public abstract class AbstractNode {

    private String nodeId;
    private String name;
    private String type;
    private double longitude;
    private double latitude;

    protected AbstractNode() {}

    protected AbstractNode(String nodeId, String name, String type,
                           double longitude, double latitude) {
        this.nodeId = nodeId;
        this.name = name;
        this.type = type;
        this.longitude = longitude;
        this.latitude = latitude;
    }

    @JsonIgnore
    public String getNodeId() {
        return nodeId;
    }

    protected void setNodeId(String nodeId) {
        this.nodeId = nodeId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public double getLongitude() {
        return longitude;
    }

    public void setLongitude(double longitude) {
        this.longitude = longitude;
    }

    public double getLatitude() {
        return latitude;
    }

    public void setLatitude(double latitude) {
        this.latitude = latitude;
    }
}
