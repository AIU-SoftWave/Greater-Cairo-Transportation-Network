package com.softwave.transportsystem.road.model;

import com.softwave.transportsystem.shared.model.AbstractEdge;
import com.softwave.transportsystem.shared.model.AbstractNode;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

/**
 * Represents an existing road in the transport network.
 */
@Entity
@Table(name = "roads")
public class Road extends AbstractEdge {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private int condition;

    public Road() {
    }

    public Road(AbstractNode fromNode, AbstractNode toNode, double distanceKm,
            int capacityVph, int condition) {
        super(fromNode, toNode, distanceKm, capacityVph);
        this.condition = condition;
    }

    public Long getId() {
        return id;
    }

    public int getCondition() {
        return condition;
    }

    public void setCondition(int condition) {
        this.condition = condition;
    }
}
