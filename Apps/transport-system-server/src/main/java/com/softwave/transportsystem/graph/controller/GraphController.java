package com.softwave.transportsystem.graph.controller;

import com.softwave.transportsystem.graph.model.MstResult;
import com.softwave.transportsystem.graph.model.ShortestPathResult;
import com.softwave.transportsystem.graph.service.DijkstraService;
import com.softwave.transportsystem.graph.service.KruskalMstService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

/**
 * REST controller that exposes the two core graph-algorithm endpoints:
 *
 * <ul>
 * <li>{@code GET /api/graph/shortest-path?from=&lt;id&gt;&amp;to=&lt;id&gt;}
 * – Dijkstra's shortest path between any two nodes using
 * {@code distance_km} as the edge weight.</li>
 * <li>{@code GET /api/graph/mst}
 * – Kruskal's Minimum Spanning Tree of the existing road network using
 * {@code distance_km} as the edge weight.</li>
 * </ul>
 *
 * <h3>Node ID format</h3>
 * Node IDs follow the same convention as the CSV data:
 * <ul>
 * <li>Numeric strings for neighborhoods, e.g. {@code "1"} (Maadi),
 * {@code "13"} (New Administrative Capital).</li>
 * <li>{@code "F"}-prefixed strings for facilities, e.g. {@code "F1"}
 * (Cairo Airport), {@code "F9"} (Qasr El Aini Hospital).</li>
 * </ul>
 */
@RestController
@RequestMapping("/api/graph")
public class GraphController {

    private final DijkstraService dijkstraService;
    private final KruskalMstService kruskalMstService;

    /**
     * Constructs the controller with its algorithm-service dependencies.
     *
     * @param dijkstraService   service that runs Dijkstra's algorithm
     * @param kruskalMstService service that runs Kruskal's MST algorithm
     */
    public GraphController(DijkstraService dijkstraService,
            KruskalMstService kruskalMstService) {
        this.dijkstraService = dijkstraService;
        this.kruskalMstService = kruskalMstService;
    }

    // endpoints

    /**
     * Finds the shortest road-distance path between two nodes using Dijkstra's
     * algorithm.
     *
     * <p>
     * <b>Examples</b>
     * </p>
     * <ul>
     * <li>{@code GET /api/graph/shortest-path?from=1&to=5} – Maadi to
     * Heliopolis</li>
     * <li>{@code GET /api/graph/shortest-path?from=F1&to=F9} – Airport to
     * hospital</li>
     * </ul>
     *
     * <p>
     * <b>Response codes</b>
     * </p>
     * <ul>
     * <li>{@code 200 OK} – path found; body contains stops and total distance.</li>
     * <li>{@code 404 Not Found} – one or both node IDs are not in the road
     * network, or the nodes belong to disconnected components.</li>
     * </ul>
     *
     * @param from source node ID (e.g. {@code "1"} or {@code "F2"})
     * @param to   destination node ID
     * @return {@link ShortestPathResult} with ordered stops and cumulative distance
     */
    @GetMapping("/shortest-path")
    public ResponseEntity<ShortestPathResult> shortestPath(
            @RequestParam String from,
            @RequestParam String to) {
        ShortestPathResult result = dijkstraService.findShortestPath(from, to);
        if (!result.isFound()) {
            return ResponseEntity.notFound().build();
        }
        return ResponseEntity.ok(result);
    }

    /**
     * Computes the Minimum Spanning Tree of the existing road network using
     * Kruskal's algorithm with {@code distance_km} as the edge weight.
     *
     * <p>
     * The MST is the set of N-1 roads (where N is the number of connected
     * nodes) that keeps the entire network connected with the minimum total
     * road length. The edges are returned in the order Kruskal's algorithm
     * selected them (ascending {@code distance_km}).
     * </p>
     *
     * @return {@link MstResult} with spanning-tree edges and summary statistics
     */
    @GetMapping("/mst")
    public MstResult minimumSpanningTree() {
        return kruskalMstService.computeMst();
    }
}
