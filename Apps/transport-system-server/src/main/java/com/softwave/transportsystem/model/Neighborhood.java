package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;

/**
 * Represents a neighborhood or district in the Greater Cairo area.
 */
@Entity
@DiscriminatorValue("NEIGHBORHOOD")
public class Neighborhood extends AbstractNode {

    private Integer population;

    public Neighborhood() {
    }

    public Neighborhood(int id, String name, int population, String type,
            double longitude, double latitude) {
        super(String.valueOf(id), name, type, longitude, latitude);
        this.population = population;
    }

    public int getId() {
        return Integer.parseInt(getNodeId());
    }

    public void setId(int id) {
        setNodeId(String.valueOf(id));
    }

    public Integer getPopulation() {
        return population;
    }

    public void setPopulation(Integer population) {
        this.population = population;
    }
}
