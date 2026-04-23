package com.softwave.transportsystem.graph.service;

import com.softwave.transportsystem.graph.shared.model.GraphEdge;
import com.softwave.transportsystem.graph.shared.service.GraphService;
import com.softwave.transportsystem.graph.shortestpath.dto.ShortestPathResult;
import com.softwave.transportsystem.graph.shortestpath.service.DijkstraService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.Mockito.when;

/**
 * Unit tests for {@link DijkstraService}.
 *
 * <p>The tests use a small synthetic graph so they run without a Spring
 * context or a database.  {@link GraphService} is mocked via Mockito and
 * configured with hand-crafted adjacency maps and name maps.</p>
 *
 * <h3>Test graph</h3>
 * <pre>
 *   1 ──2.0── 3 ──3.0── 5
 *    ╲                 /
 *     ╲────10.0───────╯
 * </pre>
 * The shortest path from node {@code "1"} to node {@code "5"} via {@code "3"}
 * costs 5.0 km, which is cheaper than the direct edge at 10.0 km.
 */
@ExtendWith(MockitoExtension.class)
class DijkstraServiceTest {

    @Mock
    private GraphService graphService;

    @InjectMocks
    private DijkstraService dijkstraService;

    /**
     * The bidirectional adjacency map for the test graph.
     * Each physical road contributes two directed entries (both directions).
     */
    private Map<String, List<GraphEdge>> adjacency;

    /** Node-ID → display-name lookup for the test graph. */
    private Map<String, String> nameMap;

    @BeforeEach
    void setUp() {
        nameMap = Map.of("1", "NodeA", "3", "NodeB", "5", "NodeC");

        // Build bidirectional adjacency (undirected treatment)
        adjacency = new HashMap<>();
        adjacency.put("1", List.of(
                new GraphEdge("1", "NodeA", "3", "NodeB", 2.0),
                new GraphEdge("1", "NodeA", "5", "NodeC", 10.0)
        ));
        adjacency.put("3", List.of(
                new GraphEdge("3", "NodeB", "1", "NodeA", 2.0),
                new GraphEdge("3", "NodeB", "5", "NodeC", 3.0)
        ));
        adjacency.put("5", List.of(
                new GraphEdge("5", "NodeC", "3", "NodeB", 3.0),
                new GraphEdge("5", "NodeC", "1", "NodeA", 10.0)
        ));

        when(graphService.buildAdjacencyMap()).thenReturn(adjacency);
        when(graphService.buildNodeNameMap()).thenReturn(nameMap);
    }

    // ------------------------------------------------------------------ path quality

    /**
     * The algorithm should prefer the two-hop route (1→3→5, total 5.0 km) over
     * the longer direct road (1→5, 10.0 km).
     */
    @Test
    void findShortestPath_prefersCheaperTwoHopRoute() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "5");

        assertTrue(result.isFound(), "Expected a path to be found");
        assertEquals(5.0, result.getTotalDistanceKm(), 0.001,
                "Expected total distance of 5.0 km via the two-hop route");
    }

    /**
     * The reconstructed path must visit the three nodes in source-to-destination
     * order: 1 → 3 → 5.
     */
    @Test
    void findShortestPath_pathOrderIsCorrect() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "5");

        assertEquals(3, result.getStops().size(), "Path should have 3 stops");
        assertEquals("1", result.getStops().get(0).getId(), "First stop should be source");
        assertEquals("3", result.getStops().get(1).getId(), "Middle stop should be relay node");
        assertEquals("5", result.getStops().get(2).getId(), "Last stop should be destination");
    }

    /**
     * Node display names from the name map must be populated in the stops.
     */
    @Test
    void findShortestPath_stopNamesArePopulated() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "5");

        assertEquals("NodeA", result.getStops().get(0).getName());
        assertEquals("NodeB", result.getStops().get(1).getName());
        assertEquals("NodeC", result.getStops().get(2).getName());
    }

    // ------------------------------------------------------------------ trivial path

    /**
     * When source equals destination the result must have a single stop, zero
     * total distance, and {@code found = true}.
     */
    @Test
    void findShortestPath_sameSourceAndDestination() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "1");

        assertTrue(result.isFound());
        assertEquals(0.0, result.getTotalDistanceKm(), 0.0);
        assertEquals(1, result.getStops().size());
        assertEquals("1", result.getStops().get(0).getId());
    }

    // ------------------------------------------------------------------ error cases

    /**
     * An unknown source node ID must return {@code found = false} with a
     * non-empty descriptive message.
     */
    @Test
    void findShortestPath_unknownSourceNode_returnsFalse() {
        ShortestPathResult result = dijkstraService.findShortestPath("99", "5");

        assertFalse(result.isFound());
        assertNotNull(result.getMessage());
        assertFalse(result.getMessage().isBlank(), "Message should explain the failure");
    }

    /**
     * An unknown destination node ID must return {@code found = false}.
     */
    @Test
    void findShortestPath_unknownDestinationNode_returnsFalse() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "99");

        assertFalse(result.isFound());
        assertNotNull(result.getMessage());
    }

    /**
     * When the two nodes exist but belong to disconnected components,
     * the algorithm must return {@code found = false}.
     *
     * <h4>Disconnected test graph</h4>
     * <pre>
     *   Component A:  1 ──2.0── 3
     *   Component B:  5 ──4.0── 7
     * </pre>
     */
    @Test
    void findShortestPath_disconnectedComponents_returnsFalse() {
        Map<String, List<GraphEdge>> disconnected = new HashMap<>();
        disconnected.put("1", List.of(new GraphEdge("1", "A", "3", "B", 2.0)));
        disconnected.put("3", List.of(new GraphEdge("3", "B", "1", "A", 2.0)));
        disconnected.put("5", List.of(new GraphEdge("5", "C", "7", "D", 4.0)));
        disconnected.put("7", List.of(new GraphEdge("7", "D", "5", "C", 4.0)));

        when(graphService.buildAdjacencyMap()).thenReturn(disconnected);
        when(graphService.buildNodeNameMap()).thenReturn(
                Map.of("1", "A", "3", "B", "5", "C", "7", "D"));

        ShortestPathResult result = dijkstraService.findShortestPath("1", "5");

        assertFalse(result.isFound(),
                "Should not find a path between nodes in different components");
    }

    // ------------------------------------------------------------------ stop list

    /**
     * On failure the stops list must be empty.
     */
    @Test
    void findShortestPath_failureResult_hasEmptyStops() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "99");

        assertFalse(result.isFound());
        assertTrue(result.getStops().isEmpty());
    }

    /**
     * On failure the total distance must be zero.
     */
    @Test
    void findShortestPath_failureResult_hasZeroDistance() {
        ShortestPathResult result = dijkstraService.findShortestPath("1", "99");

        assertEquals(0.0, result.getTotalDistanceKm(), 0.0);
    }

    // ------------------------------------------------------------------ reverse direction

    /**
     * Because the graph is undirected, the shortest path from 5 to 1 must also
     * cost 5.0 km (same route in reverse).
     */
    @Test
    void findShortestPath_reverseDirection_sameDistance() {
        ShortestPathResult fwd = dijkstraService.findShortestPath("1", "5");
        ShortestPathResult rev = dijkstraService.findShortestPath("5", "1");

        assertTrue(fwd.isFound());
        assertTrue(rev.isFound());
        assertEquals(fwd.getTotalDistanceKm(), rev.getTotalDistanceKm(), 0.001);
    }
}
