package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractEdge;
import com.softwave.transportsystem.model.Interfaces.AbstractNode;

/**
 * Represents a proposed (not yet built) road connection.
 * Data source: potential_roads.csv
 */
public class PotentialRoad extends AbstractEdge {

    private int constructionCostMEgp;

    public PotentialRoad() {
    }

    public PotentialRoad(AbstractNode fromNode, AbstractNode toNode, double distanceKm,
            int capacityVph, int constructionCostMEgp) {
        super(fromNode, toNode, distanceKm, capacityVph);
        this.constructionCostMEgp = constructionCostMEgp;
    }

    public int getConstructionCostMEgp() {
        return constructionCostMEgp;
    }

    public void setConstructionCostMEgp(int constructionCostMEgp) {
        this.constructionCostMEgp = constructionCostMEgp;
    }
}
