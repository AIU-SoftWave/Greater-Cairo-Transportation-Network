package com.softwave.transportsystem;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.LinkedHashMap;
import java.util.Map;

/**
 * Root endpoint – returns a directory of all available API endpoints.
 */
@RestController
public class HomeController {

    @GetMapping("/")
    public Map<String, Object> index() {
        Map<String, Object> info = new LinkedHashMap<>();
        info.put("app", "Greater Cairo Transportation Network API");
        info.put("version", "2.0.0");

        Map<String, String> endpoints = new LinkedHashMap<>();

        // --- Data endpoints ---

        // Neighborhoods
        endpoints.put("GET /api/neighborhoods",        "List all districts");
        endpoints.put("GET /api/neighborhoods/{id}",   "Get district by numeric ID");

        // Facilities
        endpoints.put("GET /api/facilities",           "List all facilities");
        endpoints.put("GET /api/facilities/{id}",      "Get facility by ID (e.g. F9)");

        // Roads
        endpoints.put("GET /api/roads",                "List all existing roads");
        endpoints.put("GET /api/roads/{id}",           "Get existing road by numeric ID");

        // Potential Roads
        endpoints.put("GET /api/potential-roads",      "List all proposed road segments");
        endpoints.put("GET /api/potential-roads/{id}", "Get proposed road by numeric ID");

        // Traffic
        endpoints.put("GET /api/traffic",              "List all traffic patterns");
        endpoints.put("GET /api/traffic/{id}",         "Get traffic pattern by numeric ID");

        // Metro
        endpoints.put("GET /api/metro",                "List all metro lines");
        endpoints.put("GET /api/metro/{id}",           "Get metro line by ID (e.g. M1)");

        // Bus
        endpoints.put("GET /api/bus",                  "List all bus routes");
        endpoints.put("GET /api/bus/{id}",             "Get bus route by ID (e.g. B3)");

        // Transit Demand
        endpoints.put("GET /api/demand",               "List all OD demand records");
        endpoints.put("GET /api/demand/{id}",          "Get OD demand record by numeric ID");

        // --- Implemented algorithm endpoints ---

        endpoints.put("GET /api/graph/shortest-path?from={id}&to={id}",
                "[IMPLEMENTED] Dijkstra shortest path (weight = distance_km)");
        endpoints.put("GET /api/graph/mst",
                "[IMPLEMENTED] Kruskal Minimum Spanning Tree of existing roads (weight = distance_km)");

        // --- Placeholder algorithm endpoints (not yet implemented) ---

        endpoints.put("GET /api/graph/astar?from={id}&to={id}",
                "[PLACEHOLDER] A* emergency routing (heuristic = straight-line distance)");
        endpoints.put("GET /api/graph/time-varying-shortest-path?from={id}&to={id}&timeSlot={slot}",
                "[PLACEHOLDER] Time-varying Dijkstra (weight = distance * volume/capacity). timeSlot: MORNING|AFTERNOON|EVENING|NIGHT");
        endpoints.put("GET /api/graph/prim-mst",
                "[PLACEHOLDER] Prim's MST on potential road network (weight = construction_cost)");
        endpoints.put("GET /api/traffic/signal-timing?timeSlot={slot}",
                "[PLACEHOLDER] Greedy traffic-signal timing. timeSlot: MORNING|AFTERNOON|EVENING|NIGHT");
        endpoints.put("GET /api/roads/maintenance-plan?budget={millions}",
                "[PLACEHOLDER] DP road-maintenance budget allocation (0/1 Knapsack)");
        endpoints.put("GET /api/bus/fleet-optimisation",
                "[PLACEHOLDER] DP bus fleet scheduling optimisation");
        endpoints.put("GET /api/metro/frequency-optimisation",
                "[PLACEHOLDER] DP metro frequency scheduling optimisation");

        info.put("endpoints", endpoints);
        return info;
    }
}
