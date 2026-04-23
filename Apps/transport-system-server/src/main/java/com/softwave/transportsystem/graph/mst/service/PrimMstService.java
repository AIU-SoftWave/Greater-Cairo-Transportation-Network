package com.softwave.transportsystem.graph.mst.service;

import com.softwave.transportsystem.graph.mst.dto.PotentialRoadMstResult;
import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import com.softwave.transportsystem.graph.shared.service.GraphService;
import com.softwave.transportsystem.road.model.PotentialRoad;
import com.softwave.transportsystem.road.repository.PotentialRoadRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.PriorityQueue;
import java.util.Set;

/**
 * Prim's Minimum Spanning Tree algorithm on the
 * <em>potential</em> road network.
 *
 * <h3>Algorithm description</h3>
 * Prim's algorithm grows a spanning tree one edge at a time by always adding
 * the cheapest edge that connects a new node to the already-built tree. It
 * uses a min-heap (priority queue) keyed on edge cost, giving
 * O(E log V) time complexity.
 *
 * <h3>Difference from Kruskal's</h3>
 * Kruskal's algorithm sorts all edges globally and picks them in order;
 * Prim's algorithm grows from a single seed node. On dense graphs Prim's
 * can be faster; on sparse graphs the performance is similar. Both produce
 * the same optimal MST (or one of several optimal MSTs when edge costs
 * are equal).
 *
 * <h3>Input data</h3>
 * <ul>
 * <li>{@code potential_roads.csv} – proposed edges with
 * {@code construction_cost_million_egp} as the weight</li>
 * </ul>
 *
 * <h3>Intended use case</h3>
 * Given the set of proposed new roads, find the minimum-cost subset that
 * would connect every district and facility. The result is the cheapest
 * possible new road-construction plan.
 *
 * <h3>Implementation owner</h3>
 * Assign to a team member following the contribution guide in
 * {@code PROJECT_OVERVIEW.md}.
 */
@Service
public class PrimMstService {

    private final PotentialRoadRepository potentialRoadRepository;
    private final GraphService graphService;

    public PrimMstService(PotentialRoadRepository potentialRoadRepository,
            GraphService graphService) {
        this.potentialRoadRepository = potentialRoadRepository;
        this.graphService = graphService;
    }

    /**
     * Computes the Minimum Spanning Tree of the potential road network using
     * Prim's algorithm (weight = {@code construction_cost_million_egp}).
     *
     * @return MST/forest result over proposed roads
     */
    public PotentialRoadMstResult computeMst() {
        List<PotentialRoad> potentialRoads = potentialRoadRepository.findAll();
        Map<String, String> nodeNames = graphService.buildNodeNameMap();

        Map<String, List<PotentialRoadCandidate>> adjacency = new LinkedHashMap<>();
        Set<String> nodeIds = new LinkedHashSet<>();

        for (PotentialRoad road : potentialRoads) {
            String fromId = road.getFromNode().getNodeId();
            String toId = road.getToNode().getNodeId();
            nodeIds.add(fromId);
            nodeIds.add(toId);

            adjacency.computeIfAbsent(fromId, ignored -> new ArrayList<>())
                    .add(new PotentialRoadCandidate(fromId, toId,
                            road.getDistanceKm(), road.getConstructionCostMEgp()));
            adjacency.computeIfAbsent(toId, ignored -> new ArrayList<>())
                    .add(new PotentialRoadCandidate(toId, fromId,
                            road.getDistanceKm(), road.getConstructionCostMEgp()));
        }

        List<GraphEdge> chosenEdges = new ArrayList<>();
        Set<String> visited = new LinkedHashSet<>();
        int totalConstructionCost = 0;
        double totalDistanceKm = 0.0;

        for (String startNode : nodeIds) {
            if (visited.contains(startNode)) {
                continue;
            }

            visited.add(startNode);
            PriorityQueue<PotentialRoadCandidate> pq = new PriorityQueue<>();
            pq.addAll(adjacency.getOrDefault(startNode, List.of()));

            while (!pq.isEmpty()) {
                PotentialRoadCandidate next = pq.poll();
                if (visited.contains(next.toId())) {
                    continue;
                }

                visited.add(next.toId());
                chosenEdges.add(new GraphEdge(
                        next.fromId(),
                        nodeNames.getOrDefault(next.fromId(), next.fromId()),
                        next.toId(),
                        nodeNames.getOrDefault(next.toId(), next.toId()),
                        next.distanceKm()));
                totalConstructionCost += next.constructionCostMEgp();
                totalDistanceKm += next.distanceKm();

                for (PotentialRoadCandidate candidate : adjacency.getOrDefault(next.toId(), List.of())) {
                    if (!visited.contains(candidate.toId())) {
                        pq.offer(candidate);
                    }
                }
            }
        }

        boolean connected = nodeIds.isEmpty() || chosenEdges.size() == nodeIds.size() - 1;
        String message = connected
                ? "Potential-road minimum spanning tree computed successfully."
                : "Potential road network is disconnected; returned the minimum spanning forest.";

        return new PotentialRoadMstResult(chosenEdges, totalConstructionCost, totalDistanceKm,
                nodeIds.size(), chosenEdges.size(), connected, message);
    }

    private record PotentialRoadCandidate(String fromId, String toId,
            double distanceKm, int constructionCostMEgp)
            implements Comparable<PotentialRoadCandidate> {

        @Override
        public int compareTo(PotentialRoadCandidate other) {
            return Integer.compare(this.constructionCostMEgp, other.constructionCostMEgp);
        }
    }
}
