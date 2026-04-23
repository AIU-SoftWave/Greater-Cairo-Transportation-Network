package com.softwave.transportsystem.graph.timevarying.service;

import com.softwave.transportsystem.graph.shared.dto.GraphNodeSummary;
import com.softwave.transportsystem.graph.shared.service.GraphService;
import com.softwave.transportsystem.graph.timevarying.dto.CongestedPathResult;
import com.softwave.transportsystem.road.model.Road;
import com.softwave.transportsystem.road.repository.RoadRepository;
import com.softwave.transportsystem.traffic.model.TrafficPattern;
import com.softwave.transportsystem.traffic.model.TrafficTimeSlot;
import com.softwave.transportsystem.traffic.repository.TrafficPatternRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.PriorityQueue;

/**
 * Congestion-aware Dijkstra shortest-path algorithm.
 *
 * <h3>Algorithm description</h3>
 * Standard Dijkstra is run with a <em>dynamic edge weight</em> instead of the
 * static {@code distance_km}. The effective travel-cost for a road segment
 * changes according to the time of day:
 * <pre>
 *   effective_cost = distance_km × (volume_vph / capacity_vph)
 * </pre>
 * At peak hours (morning 07-09, evening 16-19) roads operating near capacity
 * receive a much higher cost than the same roads at night, pushing the
 * algorithm towards less-congested alternatives.
 *
 * <h3>Input data</h3>
 * <ul>
 * <li>{@code existing_roads.csv} – {@code distance_km}, {@code capacity_vph}</li>
 * <li>{@code traffic_patterns.csv} – {@code morning_peak_vph},
 * {@code afternoon_vph}, {@code evening_peak_vph}, {@code night_vph}</li>
 * </ul>
 *
 * <h3>Time-of-day slots</h3>
 * <ul>
 * <li>{@code MORNING}   – 07:00–09:00</li>
 * <li>{@code AFTERNOON} – 12:00–14:00</li>
 * <li>{@code EVENING}   – 16:00–19:00</li>
 * <li>{@code NIGHT}     – 22:00–05:00</li>
 * </ul>
 *
 * <h3>Implementation owner</h3>
 * Assign to a team member following the contribution guide in
 * {@code PROJECT_OVERVIEW.md}.
 */
@Service
public class TimeVaryingDijkstraService {

    private final GraphService graphService;
    private final RoadRepository roadRepository;
    private final TrafficPatternRepository trafficPatternRepository;

    public TimeVaryingDijkstraService(GraphService graphService,
            RoadRepository roadRepository,
            TrafficPatternRepository trafficPatternRepository) {
        this.graphService = graphService;
        this.roadRepository = roadRepository;
        this.trafficPatternRepository = trafficPatternRepository;
    }

    /**
     * Finds the shortest congestion-weighted path between two nodes for a
     * given time-of-day slot.
     *
     * @param fromId   source node ID
     * @param toId     destination node ID
     * @param timeSlot one of {@code MORNING}, {@code AFTERNOON},
     *                 {@code EVENING}, or {@code NIGHT}
     * @return congestion-aware path result
     */
    public CongestedPathResult findCongestedPath(String fromId, String toId, String timeSlot) {
        TrafficTimeSlot slot;
        try {
            slot = TrafficTimeSlot.from(timeSlot);
        } catch (IllegalArgumentException exception) {
            return CongestedPathResult.invalidRequest(exception.getMessage());
        }

        Map<String, String> nameMap = graphService.buildNodeNameMap();
        if (!nameMap.containsKey(fromId)) {
            return CongestedPathResult.notFound("Source node '" + fromId + "' was not found.", slot);
        }
        if (!nameMap.containsKey(toId)) {
            return CongestedPathResult.notFound("Destination node '" + toId + "' was not found.", slot);
        }
        if (fromId.equals(toId)) {
            return CongestedPathResult.found(
                    List.of(new GraphNodeSummary(fromId, nameMap.getOrDefault(fromId, fromId))),
                    0.0, 0.0, slot);
        }

        Map<String, Integer> trafficByRoadId = buildTrafficVolumeMap(slot);
        Map<String, List<CongestedEdge>> adjacency = buildCongestedAdjacency(trafficByRoadId);

        return runDijkstra(adjacency, nameMap, fromId, toId, slot);
    }

    private Map<String, Integer> buildTrafficVolumeMap(TrafficTimeSlot slot) {
        Map<String, Integer> trafficByRoadId = new HashMap<>();
        for (TrafficPattern pattern : trafficPatternRepository.findAll()) {
            trafficByRoadId.put(pattern.getRoadId(), switch (slot) {
                case MORNING -> pattern.getMorningPeakVph();
                case AFTERNOON -> pattern.getAfternoonVph();
                case EVENING -> pattern.getEveningPeakVph();
                case NIGHT -> pattern.getNightVph();
            });
        }
        return trafficByRoadId;
    }

    private Map<String, List<CongestedEdge>> buildCongestedAdjacency(Map<String, Integer> trafficByRoadId) {
        Map<String, List<CongestedEdge>> adjacency = new LinkedHashMap<>();

        for (Road road : roadRepository.findAll()) {
            String fromId = road.getFromNode().getNodeId();
            String toId = road.getToNode().getNodeId();
            int trafficVolume = trafficByRoadId.getOrDefault(road.asRoadId(), 0);
            double effectiveCost = computeEffectiveCost(road.getDistanceKm(),
                    trafficVolume, road.getCapacityVph());

            adjacency.computeIfAbsent(fromId, ignored -> new ArrayList<>())
                    .add(new CongestedEdge(toId, road.getDistanceKm(), effectiveCost));
            adjacency.computeIfAbsent(toId, ignored -> new ArrayList<>())
                    .add(new CongestedEdge(fromId, road.getDistanceKm(), effectiveCost));
        }

        return adjacency;
    }

    private double computeEffectiveCost(double distanceKm, int trafficVolume, int capacityVph) {
        if (capacityVph <= 0) {
            return Double.MAX_VALUE;
        }
        double congestionMultiplier = Math.max(0.1, (double) trafficVolume / capacityVph);
        return distanceKm * congestionMultiplier;
    }

    private CongestedPathResult runDijkstra(Map<String, List<CongestedEdge>> adjacency,
            Map<String, String> nameMap,
            String fromId,
            String toId,
            TrafficTimeSlot slot) {
        Map<String, Double> bestCost = new HashMap<>();
        Map<String, Double> pathDistance = new HashMap<>();
        Map<String, String> predecessor = new HashMap<>();
        PriorityQueue<NodeCost> queue = new PriorityQueue<>();
        Map<String, Boolean> visited = new HashMap<>();

        nameMap.keySet().forEach(id -> {
            bestCost.put(id, Double.MAX_VALUE);
            pathDistance.put(id, Double.MAX_VALUE);
        });
        bestCost.put(fromId, 0.0);
        pathDistance.put(fromId, 0.0);
        queue.offer(new NodeCost(fromId, 0.0));

        while (!queue.isEmpty()) {
            NodeCost current = queue.poll();
            if (visited.getOrDefault(current.nodeId(), false)) {
                continue;
            }
            if (current.nodeId().equals(toId)) {
                break;
            }

            visited.put(current.nodeId(), true);

            for (CongestedEdge edge : adjacency.getOrDefault(current.nodeId(), List.of())) {
                double newCost = current.totalCost() + edge.effectiveCost();
                if (newCost < bestCost.getOrDefault(edge.toId(), Double.MAX_VALUE)) {
                    bestCost.put(edge.toId(), newCost);
                    pathDistance.put(edge.toId(),
                            pathDistance.get(current.nodeId()) + edge.distanceKm());
                    predecessor.put(edge.toId(), current.nodeId());
                    queue.offer(new NodeCost(edge.toId(), newCost));
                }
            }
        }

        if (bestCost.getOrDefault(toId, Double.MAX_VALUE) == Double.MAX_VALUE) {
            return CongestedPathResult.notFound(
                    "No congestion-aware path found between '" + fromId + "' and '" + toId + "'.",
                    slot);
        }

        return CongestedPathResult.found(
                reconstructPath(predecessor, nameMap, toId),
                pathDistance.get(toId),
                bestCost.get(toId),
                slot);
    }

    private List<GraphNodeSummary> reconstructPath(Map<String, String> predecessor,
            Map<String, String> nameMap,
            String toId) {
        LinkedList<GraphNodeSummary> path = new LinkedList<>();
        String current = toId;

        while (current != null) {
            path.addFirst(new GraphNodeSummary(current, nameMap.getOrDefault(current, current)));
            current = predecessor.get(current);
        }

        return path;
    }

    private record CongestedEdge(String toId, double distanceKm, double effectiveCost) {
    }

    private record NodeCost(String nodeId, double totalCost) implements Comparable<NodeCost> {

        @Override
        public int compareTo(NodeCost other) {
            return Double.compare(this.totalCost, other.totalCost);
        }
    }
}
