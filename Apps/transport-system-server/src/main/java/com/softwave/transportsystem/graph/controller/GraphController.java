package com.softwave.transportsystem.graph.controller;

import com.softwave.transportsystem.graph.astar.service.AStarService;
import com.softwave.transportsystem.graph.mst.dto.MstResult;
import com.softwave.transportsystem.graph.mst.dto.PotentialRoadMstResult;
import com.softwave.transportsystem.graph.mst.service.KruskalMstService;
import com.softwave.transportsystem.graph.mst.service.PrimMstService;
import com.softwave.transportsystem.graph.shortestpath.dto.ShortestPathResult;
import com.softwave.transportsystem.graph.shortestpath.service.DijkstraService;
import com.softwave.transportsystem.graph.timevarying.dto.CongestedPathResult;
import com.softwave.transportsystem.graph.timevarying.service.TimeVaryingDijkstraService;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

/**
 * REST controller that exposes all graph-algorithm endpoints.
 *
 * <h3>Implemented algorithms</h3>
 * <ul>
 * <li>{@code GET /api/graph/shortest-path?from=&lt;id&gt;&amp;to=&lt;id&gt;}
 * – Dijkstra's shortest path (weight = {@code distance_km}).</li>
 * <li>{@code GET /api/graph/mst}
 * – Kruskal's MST of the existing road network.</li>
 * </ul>
 *
 * <h3>Placeholder algorithms (not yet implemented)</h3>
 * <ul>
 * <li>{@code GET /api/graph/astar?from=&lt;id&gt;&amp;to=&lt;id&gt;}
 * – A* emergency routing.</li>
 * <li>{@code GET /api/graph/time-varying-shortest-path?from=&lt;id&gt;&amp;to=&lt;id&gt;&amp;timeSlot=&lt;slot&gt;}
 * – Congestion-aware shortest path.</li>
 * <li>{@code GET /api/graph/prim-mst}
 * – Prim's MST on the potential road network.</li>
 * </ul>
 *
 * <h3>Node ID format</h3>
 * Numeric strings for neighborhoods (e.g. {@code "1"} = Maadi) and
 * {@code "F"}-prefixed strings for facilities (e.g. {@code "F9"} = hospital).
 */
@RestController
@RequestMapping("/api/graph")
public class GraphController {

    private final DijkstraService dijkstraService;
    private final KruskalMstService kruskalMstService;
    private final AStarService aStarService;
    private final TimeVaryingDijkstraService timeVaryingDijkstraService;
    private final PrimMstService primMstService;

    /**
     * Constructs the controller with all graph-algorithm service dependencies.
     *
     * @param dijkstraService            service that runs Dijkstra's algorithm
     * @param kruskalMstService          service that runs Kruskal's MST algorithm
     * @param aStarService               placeholder for A* emergency routing
     * @param timeVaryingDijkstraService placeholder for congestion-aware routing
     * @param primMstService             placeholder for Prim's MST algorithm
     */
    public GraphController(DijkstraService dijkstraService,
            KruskalMstService kruskalMstService,
            AStarService aStarService,
            TimeVaryingDijkstraService timeVaryingDijkstraService,
            PrimMstService primMstService) {
        this.dijkstraService = dijkstraService;
        this.kruskalMstService = kruskalMstService;
        this.aStarService = aStarService;
        this.timeVaryingDijkstraService = timeVaryingDijkstraService;
        this.primMstService = primMstService;
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
     * <li>{@code 404 Not Found} – one or both node IDs are unknown, or the
     * nodes belong to disconnected components.</li>
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
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(result);
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

    // ------------------------------------------------------------------ placeholders

    /**
     * A* emergency routing.
     *
     * <p>Uses straight-line distance as the heuristic and road distance as the
     * actual path weight.</p>
     *
     * @param from source node ID
     * @param to   destination node ID (typically a medical facility)
     * @return placeholder path result
     */
    @GetMapping("/astar")
    public ResponseEntity<ShortestPathResult> aStarEmergencyPath(
            @RequestParam String from,
            @RequestParam String to) {
        ShortestPathResult result = aStarService.findEmergencyPath(from, to);
        if (!result.isFound()) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(result);
        }
        return ResponseEntity.ok(result);
    }

    /**
     * Time-varying (congestion-aware) Dijkstra shortest path.
     *
     * <p>Uses {@code distance_km * (volume_vph / capacity_vph)} as the edge
     * cost for the requested time slot.</p>
     *
     * @param from     source node ID
     * @param to       destination node ID
     * @param timeSlot one of {@code MORNING}, {@code AFTERNOON},
     *                 {@code EVENING}, {@code NIGHT}
     * @return congestion-aware path result
     */
    @GetMapping("/time-varying-shortest-path")
    public ResponseEntity<CongestedPathResult> timeVaryingShortestPath(
            @RequestParam String from,
            @RequestParam String to,
            @RequestParam String timeSlot) {
        CongestedPathResult result = timeVaryingDijkstraService.findCongestedPath(from, to, timeSlot);
        if (!result.isValidRequest()) {
            return ResponseEntity.badRequest().body(result);
        }
        if (!result.isFound()) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(result);
        }
        return ResponseEntity.ok(result);
    }

    /**
     * Prim's MST on the potential road network.
     *
     * <p>Uses {@code construction_cost_million_egp} as the edge weight and
     * returns the MST or spanning forest of the proposed network.</p>
     *
     * @return minimum-cost proposed-road network result
     */
    @GetMapping("/prim-mst")
    public PotentialRoadMstResult primMinimumSpanningTree() {
        return primMstService.computeMst();
    }
}
