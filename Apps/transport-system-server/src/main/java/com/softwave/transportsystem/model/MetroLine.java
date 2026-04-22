package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractNode;

import java.util.List;

/**
 * Represents one of Cairo's metro lines.
 * Data source: metro_lines.csv
 *
 * Fields:
 * lineId – Short identifier: "M1", "M2", "M3"
 * name – Descriptive name (e.g. "Line 1 (Helwan-New Marg)")
 * stations – Ordered list of nodes that the line passes through
 * dailyPassengers – Average number of passengers per day
 */
public class MetroLine {

    private String lineId;
    private String name;
    private List<AbstractNode> stations;
    private int dailyPassengers;

    public MetroLine() {
    }

    public MetroLine(String lineId, String name, List<AbstractNode> stations, int dailyPassengers) {
        this.lineId = lineId;
        this.name = name;
        this.stations = List.copyOf(stations);
        this.dailyPassengers = dailyPassengers;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

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
        this.stations = List.copyOf(stations);
    }

    public int getDailyPassengers() {
        return dailyPassengers;
    }

    public void setDailyPassengers(int p) {
        this.dailyPassengers = p;
    }

    public boolean servesNode(String nodeId) {
        return stations.stream().anyMatch(node -> node.getNodeId().equals(nodeId));
    }
}
