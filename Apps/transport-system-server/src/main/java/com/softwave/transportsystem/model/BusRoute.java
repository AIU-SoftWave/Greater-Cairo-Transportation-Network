package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.JoinTable;
import jakarta.persistence.ManyToMany;
import jakarta.persistence.OrderColumn;
import jakarta.persistence.Table;

import java.util.ArrayList;
import java.util.List;

/**
 * Represents a bus route in Cairo's public transit network.
 */
@Entity
@Table(name = "bus_routes")
public class BusRoute {

    @Id
    @Column(nullable = false, updatable = false)
    private String routeId;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(
            name = "bus_route_stops",
            joinColumns = @JoinColumn(name = "route_id"),
            inverseJoinColumns = @JoinColumn(name = "node_id")
    )
    @OrderColumn(name = "stop_order")
    private List<AbstractNode> stops = new ArrayList<>();

    @Column(nullable = false)
    private int busesAssigned;

    @Column(nullable = false)
    private int dailyPassengers;

    public BusRoute() {
    }

    public BusRoute(String routeId, List<AbstractNode> stops, int busesAssigned, int dailyPassengers) {
        this.routeId = routeId;
        this.stops = new ArrayList<>(stops);
        this.busesAssigned = busesAssigned;
        this.dailyPassengers = dailyPassengers;
    }

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
        this.stops = new ArrayList<>(stops);
    }

    public int getBusesAssigned() {
        return busesAssigned;
    }

    public void setBusesAssigned(int busesAssigned) {
        this.busesAssigned = busesAssigned;
    }

    public int getDailyPassengers() {
        return dailyPassengers;
    }

    public void setDailyPassengers(int dailyPassengers) {
        this.dailyPassengers = dailyPassengers;
    }

    public boolean servesNode(String nodeId) {
        return stops.stream().anyMatch(node -> node.getNodeId().equalsIgnoreCase(nodeId));
    }
}
