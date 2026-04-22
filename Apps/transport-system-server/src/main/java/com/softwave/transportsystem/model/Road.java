package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractEdge;
import com.softwave.transportsystem.model.Interfaces.AbstractNode;

/**
 * Represents an existing road (edge) in Cairo's road network.
 * Data source: existing_roads.csv
 */
public class Road extends AbstractEdge {

    private int condition;

    public Road() {
    }

    public Road(AbstractNode fromNode, AbstractNode toNode, double distanceKm,
            int capacityVph, int condition) {
        super(fromNode, toNode, distanceKm, capacityVph);
        this.condition = condition;
    }

    public int getCondition() {
        return condition;
    }

    public void setCondition(int condition) {
        this.condition = condition;
    }
}
