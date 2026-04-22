package com.softwave.transportsystem.road.model;

import com.softwave.transportsystem.shared.model.AbstractEdge;
import com.softwave.transportsystem.shared.model.AbstractNode;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

/**
 * Represents a proposed road connection.
 */
@Entity
@Table(name = "potential_roads")
public class PotentialRoad extends AbstractEdge {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private int constructionCostMEgp;

    public PotentialRoad() {
    }

    public PotentialRoad(AbstractNode fromNode, AbstractNode toNode, double distanceKm,
            int capacityVph, int constructionCostMEgp) {
        super(fromNode, toNode, distanceKm, capacityVph);
        this.constructionCostMEgp = constructionCostMEgp;
    }

    public Long getId() {
        return id;
    }

    public int getConstructionCostMEgp() {
        return constructionCostMEgp;
    }

    public void setConstructionCostMEgp(int constructionCostMEgp) {
        this.constructionCostMEgp = constructionCostMEgp;
    }
}
