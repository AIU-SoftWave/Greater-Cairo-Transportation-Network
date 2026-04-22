package com.softwave.transportsystem.model;

/**
 * Represents a proposed (not yet built) road connection.
 * Data source: potential_roads.csv
 *
 * Used by MST algorithms to evaluate which new roads are worth building
 * based on cost vs. capacity benefit.
 *
 * Fields:
 *   fromId               – Source node ID (neighborhood int or facility "F…")
 *   toId                 – Destination node ID
 *   distanceKm           – Projected road length in kilometres
 *   capacityVph          – Projected capacity in vehicles-per-hour after construction
 *   constructionCostMEgp – Estimated construction cost in millions of Egyptian Pounds
 */
public class PotentialRoad {

    private String fromId;
    private String toId;
    private double distanceKm;
    private int capacityVph;
    private int constructionCostMEgp;

    public PotentialRoad() {}

    public PotentialRoad(String fromId, String toId, double distanceKm,
                         int capacityVph, int constructionCostMEgp) {
        this.fromId               = fromId;
        this.toId                 = toId;
        this.distanceKm           = distanceKm;
        this.capacityVph          = capacityVph;
        this.constructionCostMEgp = constructionCostMEgp;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public String getFromId()                       { return fromId; }
    public void setFromId(String fromId)            { this.fromId = fromId; }

    public String getToId()                         { return toId; }
    public void setToId(String toId)                { this.toId = toId; }

    public double getDistanceKm()                   { return distanceKm; }
    public void setDistanceKm(double d)             { this.distanceKm = d; }

    public int getCapacityVph()                     { return capacityVph; }
    public void setCapacityVph(int c)               { this.capacityVph = c; }

    public int getConstructionCostMEgp()            { return constructionCostMEgp; }
    public void setConstructionCostMEgp(int cost)   { this.constructionCostMEgp = cost; }
}
