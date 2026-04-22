package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.BusRoute;
import com.softwave.transportsystem.model.MetroLine;
import com.softwave.transportsystem.model.TransitDemand;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

/**
 * Business logic for public transit: metro lines, bus routes, and
 * origin-destination demand.
 *
 * Provides:
 *  - Listing metro lines and bus routes
 *  - Looking up a line/route by ID
 *  - Querying demand between node pairs
 *  - Ranking routes by daily ridership
 */
@Service
public class TransitService {

    private final DataLoaderService data;

    public TransitService(DataLoaderService data) {
        this.data = data;
    }

    // ── Metro ────────────────────────────────────────────────────────────────

    /** Returns all metro lines. */
    public List<MetroLine> getAllMetroLines() {
        return data.getMetroLines();
    }

    /** Returns the metro line with the given ID (e.g. "M1"), or empty. */
    public Optional<MetroLine> getMetroLineById(String lineId) {
        return data.getMetroLines().stream()
            .filter(ml -> ml.getLineId().equalsIgnoreCase(lineId))
            .findFirst();
    }

    // ── Bus ──────────────────────────────────────────────────────────────────

    /** Returns all bus routes. */
    public List<BusRoute> getAllBusRoutes() {
        return data.getBusRoutes();
    }

    /** Returns the bus route with the given ID (e.g. "B3"), or empty. */
    public Optional<BusRoute> getBusRouteById(String routeId) {
        return data.getBusRoutes().stream()
            .filter(br -> br.getRouteId().equalsIgnoreCase(routeId))
            .findFirst();
    }

    /**
     * Returns bus routes sorted by daily passengers descending.
     * Useful for prioritising resources to highest-demand routes.
     */
    public List<BusRoute> getBusRoutesByRidershipDesc() {
        return data.getBusRoutes().stream()
            .sorted((a, b) -> Integer.compare(b.getDailyPassengers(), a.getDailyPassengers()))
            .toList();
    }

    /**
     * Returns bus routes that serve a specific node (stop) ID.
     * Useful to answer "which buses stop at Downtown Cairo (3)?".
     */
    public List<BusRoute> getBusRoutesForNode(String nodeId) {
        return data.getBusRoutes().stream()
            .filter(br -> br.getStops().contains(nodeId))
            .toList();
    }

    // ── Demand ───────────────────────────────────────────────────────────────

    /** Returns all origin-destination demand records. */
    public List<TransitDemand> getAllDemand() {
        return data.getTransitDemands();
    }

    /**
     * Returns demand records originating from the given node ID.
     * Sorted by daily passengers descending.
     */
    public List<TransitDemand> getDemandFrom(String fromId) {
        return data.getTransitDemands().stream()
            .filter(d -> d.getFromId().equals(fromId))
            .sorted((a, b) -> Integer.compare(b.getDailyPassengers(), a.getDailyPassengers()))
            .toList();
    }

    /**
     * Returns demand records destined for the given node ID.
     * Sorted by daily passengers descending.
     */
    public List<TransitDemand> getDemandTo(String toId) {
        return data.getTransitDemands().stream()
            .filter(d -> d.getToId().equals(toId))
            .sorted((a, b) -> Integer.compare(b.getDailyPassengers(), a.getDailyPassengers()))
            .toList();
    }
}
