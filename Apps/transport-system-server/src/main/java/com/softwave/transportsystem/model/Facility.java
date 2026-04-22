package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;

/**
 * Represents an important facility in Cairo (airport, hospital, university,
 * etc.).
 * Data source: facilities.csv
 */
public class Facility extends AbstractNode {

    public Facility() {
    }

    public Facility(String id, String name, String type,
            double longitude, double latitude) {
        super(id, name, type, longitude, latitude);
    }

    public String getId() {
        return getNodeId();
    }

    public void setId(String id) {
        setNodeId(id);
    }
}
