package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;

import java.util.List;

/**
 * Represents a bus route in Cairo's public transit network.
 * Data source: bus_routes.csv
 *
 * Fields:
 * routeId – Short identifier: "B1" … "B10"
 * stops – Ordered list of nodes along the route
 * busesAssigned – Number of buses currently operating this route
 * dailyPassengers – Average passengers carried per day
 */
public class BusRoute {

    private String routeId;
    private List<AbstractNode> stops;
    private int busesAssigned;
    private int dailyPassengers;

    public BusRoute() {
    }

    public BusRoute(String routeId, List<AbstractNode> stops, int busesAssigned, int dailyPassengers) {
        this.routeId = routeId;
        this.stops = List.copyOf(stops);
        this.busesAssigned = busesAssigned;
        this.dailyPassengers = dailyPassengers;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public String getRouteId() {
        return routeId;
    }

    public void setRouteId(String routeId) {
        this.routeId = routeId;
    }

    public List<AbstractNode> getStops() {
        return stops;
    }

    public void setStops(List<AbstractNode> stops) {
        this.stops = List.copyOf(stops);
    }

    public int getBusesAssigned() {
        return busesAssigned;
    }

    public void setBusesAssigned(int b) {
        this.busesAssigned = b;
    }

    public int getDailyPassengers() {
        return dailyPassengers;
    }

    public void setDailyPassengers(int p) {
        this.dailyPassengers = p;
    }

    public boolean servesNode(String nodeId) {
        return stops.stream().anyMatch(node -> node.getNodeId().equals(nodeId));
    }
}
