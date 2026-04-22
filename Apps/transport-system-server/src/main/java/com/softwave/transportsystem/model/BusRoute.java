package com.softwave.transportsystem.model;

import java.util.List;

/**
 * Represents a bus route in Cairo's public transit network.
 * Data source: bus_routes.csv
 *
 * Fields:
 *   routeId         – Short identifier: "B1" … "B10"
 *   stops           – Ordered list of node IDs along the route
 *   busesAssigned   – Number of buses currently operating this route
 *   dailyPassengers – Average passengers carried per day
 */
public class BusRoute {

    private String routeId;
    private List<String> stops;
    private int busesAssigned;
    private int dailyPassengers;

    public BusRoute() {}

    public BusRoute(String routeId, List<String> stops, int busesAssigned, int dailyPassengers) {
        this.routeId         = routeId;
        this.stops           = stops;
        this.busesAssigned   = busesAssigned;
        this.dailyPassengers = dailyPassengers;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public String getRouteId()                   { return routeId; }
    public void setRouteId(String routeId)       { this.routeId = routeId; }

    public List<String> getStops()               { return stops; }
    public void setStops(List<String> stops)     { this.stops = stops; }

    public int getBusesAssigned()                { return busesAssigned; }
    public void setBusesAssigned(int b)          { this.busesAssigned = b; }

    public int getDailyPassengers()              { return dailyPassengers; }
    public void setDailyPassengers(int p)        { this.dailyPassengers = p; }
}
