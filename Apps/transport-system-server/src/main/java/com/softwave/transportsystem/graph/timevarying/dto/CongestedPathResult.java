package com.softwave.transportsystem.graph.timevarying.dto;

import com.softwave.transportsystem.graph.shared.dto.GraphNodeSummary;
import com.softwave.transportsystem.traffic.model.TrafficTimeSlot;

import java.util.List;

/**
 * Result of congestion-aware shortest path search.
 */
public class CongestedPathResult {

    private final boolean found;
    private final boolean validRequest;
    private final List<GraphNodeSummary> stops;
    private final double totalDistanceKm;
    private final double totalTravelCost;
    private final TrafficTimeSlot timeSlot;
    private final String message;

    private CongestedPathResult(boolean found, boolean validRequest,
            List<GraphNodeSummary> stops, double totalDistanceKm,
            double totalTravelCost, TrafficTimeSlot timeSlot,
            String message) {
        this.found = found;
        this.validRequest = validRequest;
        this.stops = stops;
        this.totalDistanceKm = totalDistanceKm;
        this.totalTravelCost = totalTravelCost;
        this.timeSlot = timeSlot;
        this.message = message;
    }

    public static CongestedPathResult found(List<GraphNodeSummary> stops,
            double totalDistanceKm, double totalTravelCost,
            TrafficTimeSlot timeSlot) {
        return new CongestedPathResult(true, true, stops, totalDistanceKm,
                totalTravelCost, timeSlot, "Congestion-aware path found.");
    }

    public static CongestedPathResult notFound(String message, TrafficTimeSlot timeSlot) {
        return new CongestedPathResult(false, true, List.of(), 0.0,
                0.0, timeSlot, message);
    }

    public static CongestedPathResult invalidRequest(String message) {
        return new CongestedPathResult(false, false, List.of(), 0.0,
                0.0, null, message);
    }

    public boolean isFound() {
        return found;
    }

    public boolean isValidRequest() {
        return validRequest;
    }

    public List<GraphNodeSummary> getStops() {
        return stops;
    }

    public double getTotalDistanceKm() {
        return totalDistanceKm;
    }

    public double getTotalTravelCost() {
        return totalTravelCost;
    }

    public TrafficTimeSlot getTimeSlot() {
        return timeSlot;
    }

    public String getMessage() {
        return message;
    }
}
