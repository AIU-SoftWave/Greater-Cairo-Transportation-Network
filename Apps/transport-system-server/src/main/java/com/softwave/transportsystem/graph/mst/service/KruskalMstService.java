package com.softwave.transportsystem.graph.mst.service;

import com.softwave.transportsystem.graph.mst.dto.MstResult;
import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import com.softwave.transportsystem.graph.shared.model.GraphSnapshot;
import com.softwave.transportsystem.graph.shared.service.GraphService;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

/**
 * Implements Kruskal's Minimum Spanning Tree algorithm on the existing road
 * network, using {@code distance_km} as the edge weight.
 *
 * <h3>Algorithm summary</h3>
 * <ol>
 * <li>Collect every unique node ID that appears as an endpoint in at least
 * one road.</li>
 * <li>Sort all road edges by {@code distance_km} in ascending order.</li>
 * <li>Iterate through the sorted edges. For each edge, use the
 * Union-Find (Disjoint Set Union) structure to check whether the two
 * endpoints already belong to the same component:
 * <ul>
 * <li>If they do, adding this edge would create a cycle – skip it.</li>
 * <li>If they do not, merge the two components and include this edge
 * in the MST.</li>
 * </ul>
 * </li>
 * <li>Stop as soon as N-1 edges have been selected (where N is the number
 * of unique nodes), because a spanning tree of N nodes has exactly
 * N-1 edges.</li>
 * </ol>
 *
 * <h3>Union-Find optimisations</h3>
 * The internal {@link UnionFind} class uses both <em>path compression</em> and
 * <em>union by rank</em> to achieve nearly O(1) amortised per-operation cost,
 * giving the overall algorithm an O(E log E) time complexity (dominated by the
 * edge sort).
 *
 * <h3>Undirected graph</h3>
 * Each row of {@code existing_roads.csv} represents one undirected road.
 * {@link GraphService#buildGraphSnapshot()} exposes one canonical
 * {@link GraphEdge} per physical road, which is the correct input for
 * Kruskal's.
 *
 * <h3>Edge weight</h3>
 * {@code distance_km} is used as the spanning-tree weight, as specified in the
 * algorithm roadmap: "Minimum-cost network connecting all areas."
 */
@Service
public class KruskalMstService {

    private final GraphService graphService;

    /**
     * Constructs the service with its graph-building dependency.
     *
     * @param graphService builds the in-memory edge list from the DB
     */
    public KruskalMstService(GraphService graphService) {
        this.graphService = graphService;
    }

    // ------------------------------------------------------------------ public API

    /**
     * Computes the Minimum Spanning Tree of the existing road network.
     *
     * <p>
     * The result contains the N-1 spanning-tree edges selected by Kruskal's
     * algorithm together with their cumulative {@code distance_km}. If the
     * road network is disconnected the algorithm returns the spanning forest
     * (one spanning tree per connected component), so the edge count may be
     * less than N-1.
     * </p>
     *
     * @return {@link MstResult} with the MST edges and summary statistics
     */
    public MstResult computeMst() {
        GraphSnapshot snapshot = graphService.buildGraphSnapshot();
        List<GraphEdge> allEdges = snapshot.getEdges();
        Map<String, String> nodeNames = snapshot.getNodeNames();

        Set<String> nodeIds = new LinkedHashSet<>();
        for (GraphEdge edge : allEdges) {
            nodeIds.add(edge.getFromId());
            nodeIds.add(edge.getToId());
        }

        List<GraphEdge> sorted = new ArrayList<>(allEdges);
        sorted.sort(Comparator.comparingDouble(GraphEdge::getDistanceKm));

        UnionFind uf = new UnionFind(nodeIds);

        List<GraphEdge> mstEdges = new ArrayList<>();
        double totalDist = 0.0;
        int target = nodeIds.size() - 1;

        for (GraphEdge edge : sorted) {
            String rootFrom = uf.find(edge.getFromId());
            String rootTo = uf.find(edge.getToId());

            if (rootFrom.equals(rootTo)) {
                continue;
            }

            uf.union(rootFrom, rootTo);
            mstEdges.add(edge);
            totalDist += edge.getDistanceKm();

            if (mstEdges.size() == target) {
                break;
            }
        }

        List<GraphEdge> resultEdges = mstEdges.stream()
                .map(edge -> new GraphEdge(
                        edge.getFromId(),
                        nodeNames.getOrDefault(edge.getFromId(), edge.getFromId()),
                        edge.getToId(),
                        nodeNames.getOrDefault(edge.getToId(), edge.getToId()),
                        edge.getDistanceKm()))
                .toList();

        boolean connected = nodeIds.isEmpty() || mstEdges.size() == target;
        String message = connected
                ? "Minimum spanning tree computed successfully."
                : "Road network is disconnected; returned the minimum spanning forest.";

        return new MstResult(resultEdges, totalDist, nodeIds.size(), mstEdges.size(),
                connected, message);
    }

    // ------------------------------------------------------------------ Union-Find

    /**
     * Disjoint Set Union (Union-Find) data structure with path compression and
     * union by rank.
     *
     * <ul>
     * <li><b>Path compression</b> – on every {@link #find} call, each node on
     * the path to the root is re-pointed directly at the root, flattening
     * the tree for future lookups.</li>
     * <li><b>Union by rank</b> – the smaller-rank tree is always attached
     * beneath the larger-rank root, keeping trees shallow.</li>
     * </ul>
     *
     * Combined, these optimisations yield an amortised nearly-O(1) cost per
     * operation (inverse-Ackermann O(α(n))).
     */
    private static class UnionFind {

        /** Each node starts as its own parent (singleton component). */
        private final Map<String, String> parent = new HashMap<>();

        /** Upper-bound on the height of each component tree. */
        private final Map<String, Integer> rank = new HashMap<>();

        /**
         * Initialises one singleton component per node in {@code nodes}.
         *
         * @param nodes the full set of graph node IDs
         */
        UnionFind(Set<String> nodes) {
            for (String node : nodes) {
                parent.put(node, node);
                rank.put(node, 0);
            }
        }

        /**
         * Returns the root representative of the component containing
         * {@code node}, applying path compression along the way.
         *
         * @param node any node ID present in this structure
         * @return the canonical root of {@code node}'s component
         */
        String find(String node) {
            if (!parent.get(node).equals(node)) {
                parent.put(node, find(parent.get(node)));
            }
            return parent.get(node);
        }

        /**
         * Merges the two components represented by {@code rootA} and
         * {@code rootB}. Callers must pass the roots returned by
         * {@link #find}, not arbitrary node IDs.
         *
         * @param rootA root of the first component
         * @param rootB root of the second component
         */
        void union(String rootA, String rootB) {
            if (rootA.equals(rootB)) {
                return;
            }

            int rankA = rank.get(rootA);
            int rankB = rank.get(rootB);

            if (rankA < rankB) {
                parent.put(rootA, rootB);
            } else if (rankA > rankB) {
                parent.put(rootB, rootA);
            } else {
                parent.put(rootB, rootA);
                rank.put(rootA, rankA + 1);
            }
        }
    }
}
