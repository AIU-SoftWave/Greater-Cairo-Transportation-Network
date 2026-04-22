package com.softwave.transportsystem.transit.model;

import com.softwave.transportsystem.shared.model.AbstractNode;
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
 * Represents one of Cairo's metro lines.
 */
@Entity
@Table(name = "metro_lines")
public class MetroLine {

    @Id
    @Column(nullable = false, updatable = false)
    private String lineId;

    @Column(nullable = false)
    private String name;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(
            name = "metro_line_stations",
            joinColumns = @JoinColumn(name = "line_id"),
            inverseJoinColumns = @JoinColumn(name = "node_id")
    )
    @OrderColumn(name = "station_order")
    private List<AbstractNode> stations = new ArrayList<>();

    @Column(nullable = false)
    private int dailyPassengers;

    public MetroLine() {
    }

    public MetroLine(String lineId, String name, List<AbstractNode> stations, int dailyPassengers) {
        this.lineId = lineId;
        this.name = name;
        this.stations = new ArrayList<>(stations);
        this.dailyPassengers = dailyPassengers;
    }

    public String getLineId() {
        return lineId;
    }

    public void setLineId(String lineId) {
        this.lineId = lineId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public List<AbstractNode> getStations() {
        return stations;
    }

    public void setStations(List<AbstractNode> stations) {
        this.stations = new ArrayList<>(stations);
    }

    public int getDailyPassengers() {
        return dailyPassengers;
    }

    public void setDailyPassengers(int dailyPassengers) {
        this.dailyPassengers = dailyPassengers;
    }
}
