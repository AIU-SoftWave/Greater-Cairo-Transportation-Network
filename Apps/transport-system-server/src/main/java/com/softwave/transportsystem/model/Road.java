package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractEdge;
import com.softwave.transportsystem.model.Interfaces.AbstractNode;
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

    public int getCondition() {
        return condition;
    }

    public void setCondition(int condition) {
        this.condition = condition;
    }
}
