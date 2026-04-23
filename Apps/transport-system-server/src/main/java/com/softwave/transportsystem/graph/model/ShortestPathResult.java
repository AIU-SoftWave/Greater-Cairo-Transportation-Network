package com.softwave.transportsystem.graph.model;

import java.util.List;

/**
 * Encapsulates the result of Dijkstra's shortest-path search between two nodes
 * in the existing road network.
 *
 * <h3>Success case</h3>
 * When {@link #isFound()} is {@code true}:
 * <ul>
 * <li>{@link #getStops()} contains the ordered sequence of nodes from the
 * source to the destination (inclusive of both endpoints).</li>
 * <li>{@link #getTotalDistanceKm()} holds the sum of all road distances
 * along the chosen route.</li>
 * </ul>
 *
 * <h3>Failure case</h3>
 * When {@link #isFound()} is {@code false}:
 * <ul>
 * <li>{@link #getStops()} is an empty list.</li>
 * <li>{@link #getTotalDistanceKm()} is {@code 0.0}.</li>
 * <li>{@link #getMessage()} explains the reason (unknown node ID, no
 * connecting path, etc.).</li>
 * </ul>
 *
 * <p>
 * Instances are created exclusively through the static factory methods
 * {@link #found} and {@link #notFound}.
 * </p>
 */
public class ShortestPathResult {

    //  inner type

    /**
     * A single node visited along the shortest route.
     * Carries both the raw node ID (as stored in the database) and its
     * human-readable display name.
     */
    public static class PathStop {

        /** Node ID, e.g. {@code "1"} for Maadi or {@code "F2"} for Ramses station. */
        private final String id;

        /** Human-readable district / facility name. */
        private final String name;

        public PathStop(String id, String name) {
            this.id = id;
            this.name = name;
        }

        public String getId() {
            return id;
        }

        public String getName() {
            return name;
        }
    }

    // ------------------------------------------------------------------ fields

    /** {@code true} if a path was found between the requested endpoints. */
    private final boolean found;

    /**
     * Ordered list of nodes along the shortest route, starting at the source
     * and ending at the destination. Empty when {@link #found} is {@code false}.
     */
    private final List<PathStop> stops;

    /** Total road distance in kilometres along the shortest route. */
    private final double totalDistanceKm;

    /**
     * Informational message. Always {@code "Path found."} on success; contains
     * an explanation on failure.
     */
    private final String message;

    // ------------------------------------------------------------------
    // constructor

    private ShortestPathResult(boolean found, List<PathStop> stops,
            double totalDistanceKm, String message) {
        this.found = found;
        this.stops = stops;
        this.totalDistanceKm = totalDistanceKm;
        this.message = message;
    }

    // ------------------------------------------------------------------ factories

    /**
     * Creates a successful result with the fully reconstructed path.
     *
     * @param stops           ordered nodes from source to destination
     * @param totalDistanceKm cumulative road distance along the route
     * @return successful {@code ShortestPathResult}
     */
    public static ShortestPathResult found(List<PathStop> stops, double totalDistanceKm) {
        return new ShortestPathResult(true, stops, totalDistanceKm, "Path found.");
    }

    /**
     * Creates a failure result, e.g. when a node ID is not in the road network
     * or the two nodes belong to disconnected components.
     *
     * @param message human-readable explanation of the failure
     * @return failure {@code ShortestPathResult}
     */
    public static ShortestPathResult notFound(String message) {
        return new ShortestPathResult(false, List.of(), 0.0, message);
    }

    // ------------------------------------------------------------------ getters

    public boolean isFound() {
        return found;
    }

    public List<PathStop> getStops() {
        return stops;
    }

    public double getTotalDistanceKm() {
        return totalDistanceKm;
    }

    public String getMessage() {
        return message;
    }
}
