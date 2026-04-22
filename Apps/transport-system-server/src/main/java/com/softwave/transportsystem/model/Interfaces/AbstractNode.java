package com.softwave.transportsystem.model.Interfaces;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorColumn;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Inheritance;
import jakarta.persistence.InheritanceType;
import jakarta.persistence.Table;

/**
 * Shared base type for graph endpoints such as neighborhoods and facilities.
 */
@Entity
@Table(name = "nodes")
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(name = "node_kind")
public abstract class AbstractNode {

    @Id
    @Column(name = "node_id", nullable = false, updatable = false)
    private String nodeId;

    @Column(nullable = false)
    private String name;

    @Column(nullable = false)
    private String type;

    @Column(nullable = false)
    private double longitude;

    @Column(nullable = false)
    private double latitude;

    protected AbstractNode() {
    }

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
