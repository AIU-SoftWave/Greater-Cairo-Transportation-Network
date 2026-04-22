package com.softwave.transportsystem.transit.model;

import com.softwave.transportsystem.shared.model.AbstractNode;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

/**
 * Records observed public-transit passenger demand between two nodes.
 */
@Entity
@Table(name = "transit_demands")
public class TransitDemand {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.EAGER, optional = false)
    @JoinColumn(name = "from_node_id", nullable = false)
    private AbstractNode fromNode;

    @ManyToOne(fetch = FetchType.EAGER, optional = false)
    @JoinColumn(name = "to_node_id", nullable = false)
    private AbstractNode toNode;

    private int dailyPassengers;

    public TransitDemand() {
    }

    public TransitDemand(AbstractNode fromNode, AbstractNode toNode, int dailyPassengers) {
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.dailyPassengers = dailyPassengers;
    }

    public Long getId() {
        return id;
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

    public int getDailyPassengers() {
        return dailyPassengers;
    }

    public void setDailyPassengers(int dailyPassengers) {
        this.dailyPassengers = dailyPassengers;
    }
}
