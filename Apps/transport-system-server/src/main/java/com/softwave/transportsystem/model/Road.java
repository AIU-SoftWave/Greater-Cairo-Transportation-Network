package com.softwave.transportsystem.model;

/**
 * Represents an existing road (edge) in Cairo's road network.
 * Data source: existing_roads.csv
 *
 * Node IDs are stored as strings because they can be either an integer
 * (neighborhood, e.g. "3") or a facility code (e.g. "F2").
 *
 * Fields:
 *   fromId      – Source node ID
 *   toId        – Destination node ID
 *   distanceKm  – Road length in kilometres
 *   capacityVph – Maximum traffic capacity in vehicles-per-hour
 *   condition   – Current road quality on a 1-10 scale (10 = perfect)
 */
public class Road {

    private String fromId;
    private String toId;
    private double distanceKm;
    private int capacityVph;
    private int condition;

    public Road() {}

    public Road(String fromId, String toId, double distanceKm,
                int capacityVph, int condition) {
        this.fromId      = fromId;
        this.toId        = toId;
        this.distanceKm  = distanceKm;
        this.capacityVph = capacityVph;
        this.condition   = condition;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public String getFromId()                  { return fromId; }
    public void setFromId(String fromId)       { this.fromId = fromId; }

    public String getToId()                    { return toId; }
    public void setToId(String toId)           { this.toId = toId; }

    public double getDistanceKm()              { return distanceKm; }
    public void setDistanceKm(double d)        { this.distanceKm = d; }

    public int getCapacityVph()                { return capacityVph; }
    public void setCapacityVph(int c)          { this.capacityVph = c; }

    public int getCondition()                  { return condition; }
    public void setCondition(int condition)    { this.condition = condition; }
}
