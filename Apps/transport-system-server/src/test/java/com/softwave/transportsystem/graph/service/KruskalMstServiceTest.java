package com.softwave.transportsystem.graph.service;

import com.softwave.transportsystem.graph.model.GraphEdge;
import com.softwave.transportsystem.graph.model.MstResult;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.Mockito.when;

/**
 * Unit tests for {@link KruskalMstService}.
 *
 * <p>The tests use a small synthetic graph so they run without a Spring
 * context or a database.  {@link GraphService} is mocked via Mockito and
 * configured with a hand-crafted edge list.</p>
 *
 * <h3>Test graph (4 nodes, 5 edges)</h3>
 * <pre>
 *   1 ──1.0── 2 ──2.0── 3 ──3.0── 4
 *    ╲              ╱         ╱
 *     ╲───4.0──────╯──5.0────╯
 * </pre>
 * Edges sorted by weight: (1,2)=1.0, (2,3)=2.0, (3,4)=3.0, (1,3)=4.0, (2,4)=5.0
 *
 * <p>Kruskal's algorithm selects edges greedily in ascending weight order:</p>
 * <ol>
 *   <li>(1,2) = 1.0 – safe, merges nodes 1 and 2</li>
 *   <li>(2,3) = 2.0 – safe, merges {1,2} with {3}</li>
 *   <li>(3,4) = 3.0 – safe, merges {1,2,3} with {4}; MST complete (3 = N-1 edges)</li>
 *   <li>(1,3) = 4.0 – skipped (1 and 3 already connected)</li>
 *   <li>(2,4) = 5.0 – skipped (2 and 4 already connected)</li>
 * </ol>
 *
 * <p>Expected MST: total 6.0 km, 3 edges, node count 4.</p>
 */
@ExtendWith(MockitoExtension.class)
class KruskalMstServiceTest {

    @Mock
    private GraphService graphService;

    @InjectMocks
    private KruskalMstService kruskalMstService;

    @BeforeEach
    void setUp() {
        // One entry per physical (undirected) road – no duplicates for Kruskal's
        List<GraphEdge> edges = List.of(
                new GraphEdge("1", "A", "2", "B", 1.0),
                new GraphEdge("1", "A", "3", "C", 4.0),
                new GraphEdge("2", "B", "3", "C", 2.0),
                new GraphEdge("2", "B", "4", "D", 5.0),
                new GraphEdge("3", "C", "4", "D", 3.0)
        );
        when(graphService.buildEdgeList()).thenReturn(edges);
    }

    // ------------------------------------------------------------------ structure

    /**
     * A spanning tree of N nodes must have exactly N-1 edges.
     */
    @Test
    void computeMst_edgeCountIsNMinus1() {
        MstResult result = kruskalMstService.computeMst();

        assertEquals(result.getNodeCount() - 1, result.getEdgeCount(),
                "MST must have exactly nodeCount - 1 edges");
    }

    /**
     * The reported node count must equal the number of unique node IDs in the
     * edge list (4 for the test graph).
     */
    @Test
    void computeMst_nodeCountMatchesUniqueNodes() {
        MstResult result = kruskalMstService.computeMst();

        assertEquals(4, result.getNodeCount());
    }

    /**
     * The MST edge list size must match the reported edge count.
     */
    @Test
    void computeMst_edgeListSizeMatchesEdgeCount() {
        MstResult result = kruskalMstService.computeMst();

        assertEquals(result.getEdgeCount(), result.getEdges().size());
    }

    // ------------------------------------------------------------------ weight

    /**
     * The total weight of the MST must equal the sum of its edge distances.
     * For this graph the optimal total is 1.0 + 2.0 + 3.0 = 6.0 km.
     */
    @Test
    void computeMst_totalDistanceIsMinimal() {
        MstResult result = kruskalMstService.computeMst();

        assertEquals(6.0, result.getTotalDistanceKm(), 0.001,
                "MST total distance should be 6.0 km (edges of weight 1, 2, 3)");
    }

    /**
     * The summed edge weights must be consistent with the reported total.
     */
    @Test
    void computeMst_reportedTotalMatchesSumOfEdges() {
        MstResult result = kruskalMstService.computeMst();

        double sumFromEdges = result.getEdges().stream()
                .mapToDouble(GraphEdge::getDistanceKm)
                .sum();

        assertEquals(sumFromEdges, result.getTotalDistanceKm(), 0.001);
    }

    // ------------------------------------------------------------------ edge selection

    /**
     * Kruskal's must include the three cheapest non-cycle-forming edges
     * (weights 1.0, 2.0, 3.0) and must exclude the more expensive ones
     * (weights 4.0 and 5.0).
     */
    @Test
    void computeMst_selectsCheapestEdges() {
        MstResult result = kruskalMstService.computeMst();

        Set<Double> selectedWeights = result.getEdges().stream()
                .map(GraphEdge::getDistanceKm)
                .collect(Collectors.toSet());

        assertTrue(selectedWeights.contains(1.0), "MST must include edge of weight 1.0");
        assertTrue(selectedWeights.contains(2.0), "MST must include edge of weight 2.0");
        assertTrue(selectedWeights.contains(3.0), "MST must include edge of weight 3.0");
    }

    /**
     * The more expensive edges (4.0 and 5.0) form cycles in the MST candidate
     * set and must therefore be excluded.
     */
    @Test
    void computeMst_excludesRedundantEdges() {
        MstResult result = kruskalMstService.computeMst();

        Set<Double> selectedWeights = result.getEdges().stream()
                .map(GraphEdge::getDistanceKm)
                .collect(Collectors.toSet());

        assertFalse(selectedWeights.contains(4.0),
                "Edge of weight 4.0 creates a cycle and must be excluded");
        assertFalse(selectedWeights.contains(5.0),
                "Edge of weight 5.0 creates a cycle and must be excluded");
    }

    // ------------------------------------------------------------------ disconnected graph

    /**
     * For a disconnected graph (spanning forest), Kruskal's must still return
     * a valid result with fewer than N-1 edges.
     *
     * <h4>Disconnected test graph</h4>
     * <pre>
     *   Component A:  1 ──1.0── 2
     *   Component B:  3 ──2.0── 4
     * </pre>
     * Expected: 2 MST edges (one spanning tree per component), 4 nodes.
     */
    @Test
    void computeMst_disconnectedGraph_returnsSpanningForest() {
        List<GraphEdge> disconnectedEdges = List.of(
                new GraphEdge("1", "A", "2", "B", 1.0),
                new GraphEdge("3", "C", "4", "D", 2.0)
        );
        when(graphService.buildEdgeList()).thenReturn(disconnectedEdges);

        MstResult result = kruskalMstService.computeMst();

        // 4 nodes in 2 components → 2 spanning-tree edges (N - components = 4 - 2)
        assertEquals(4, result.getNodeCount());
        assertEquals(2, result.getEdgeCount());
        assertEquals(3.0, result.getTotalDistanceKm(), 0.001);
    }

    // ------------------------------------------------------------------ single edge

    /**
     * A graph with exactly two nodes and one edge must produce a single-edge MST.
     */
    @Test
    void computeMst_singleEdgeGraph() {
        List<GraphEdge> singleEdge = List.of(
                new GraphEdge("1", "A", "2", "B", 7.5)
        );
        when(graphService.buildEdgeList()).thenReturn(singleEdge);

        MstResult result = kruskalMstService.computeMst();

        assertEquals(2,   result.getNodeCount());
        assertEquals(1,   result.getEdgeCount());
        assertEquals(7.5, result.getTotalDistanceKm(), 0.001);
    }
}
