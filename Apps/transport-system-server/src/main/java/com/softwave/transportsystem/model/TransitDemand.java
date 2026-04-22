package com.softwave.transportsystem.model;

/**
 * Records observed public-transit passenger demand between two nodes.
 * Data source: transit_demand.csv
 *
 * Fields:
 *   fromId          – Origin node ID
 *   toId            – Destination node ID
 *   dailyPassengers – Average daily passenger trips on this origin-destination pair
 */
public class TransitDemand {

    private String fromId;
    private String toId;
    private int dailyPassengers;

    public TransitDemand() {}

    public TransitDemand(String fromId, String toId, int dailyPassengers) {
        this.fromId          = fromId;
        this.toId            = toId;
        this.dailyPassengers = dailyPassengers;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public String getFromId()                       { return fromId; }
    public void setFromId(String fromId)            { this.fromId = fromId; }

    public String getToId()                         { return toId; }
    public void setToId(String toId)                { this.toId = toId; }

    public int getDailyPassengers()                 { return dailyPassengers; }
    public void setDailyPassengers(int p)           { this.dailyPassengers = p; }
}
