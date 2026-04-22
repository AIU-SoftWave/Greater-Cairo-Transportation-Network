package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;

/**
 * Represents a neighborhood or district in the Greater Cairo area.
 * Data source: nodes.csv
 */
public class Neighborhood extends AbstractNode {

    private int population;

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

    public int getPopulation() {
        return population;
    }

    public void setPopulation(int population) {
        this.population = population;
    }
}
