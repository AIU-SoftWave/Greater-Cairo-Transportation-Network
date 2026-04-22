package com.softwave.transportsystem.controllers;

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
        info.put("version", "1.0.0");

        Map<String, String> endpoints = new LinkedHashMap<>();

        // Neighborhoods
        endpoints.put("GET /api/neighborhoods",                      "List all districts");
        endpoints.put("GET /api/neighborhoods/{id}",                 "Get district by numeric ID");
        endpoints.put("GET /api/neighborhoods?type=Residential",     "Filter districts by type");
        endpoints.put("GET /api/neighborhoods/top-population",       "Districts sorted by population");

        // Facilities
        endpoints.put("GET /api/facilities",                         "List all facilities");
        endpoints.put("GET /api/facilities/{id}",                    "Get facility by ID (e.g. F9)");
        endpoints.put("GET /api/facilities?type=Medical",            "Filter facilities by type");

        // Roads
        endpoints.put("GET /api/roads",                              "List all existing roads");
        endpoints.put("GET /api/roads?node=3",                       "Roads connected to a node");
        endpoints.put("GET /api/roads/poor-condition?maxCondition=5","Roads needing maintenance");
        endpoints.put("GET /api/roads/potential",                    "Proposed new road segments");
        endpoints.put("GET /api/roads/potential/by-cost",            "Proposed roads sorted by cost");

        // Traffic
        endpoints.put("GET /api/traffic",                            "All time-of-day traffic patterns");
        endpoints.put("GET /api/traffic/{roadId}",                   "Pattern for one road (e.g. 1-3)");
        endpoints.put("GET /api/traffic/morning-congestion",         "Congested roads in AM peak");
        endpoints.put("GET /api/traffic/evening-congestion",         "Congested roads in PM peak");

        // Transit
        endpoints.put("GET /api/transit/metro",                      "All metro lines");
        endpoints.put("GET /api/transit/metro/{lineId}",             "One metro line (e.g. M1)");
        endpoints.put("GET /api/transit/bus",                        "All bus routes");
        endpoints.put("GET /api/transit/bus/{routeId}",              "One bus route (e.g. B3)");
        endpoints.put("GET /api/transit/bus?node=3",                 "Bus routes serving a node");
        endpoints.put("GET /api/transit/bus/top-ridership",          "Bus routes by daily ridership");
        endpoints.put("GET /api/transit/demand",                     "All OD demand records");
        endpoints.put("GET /api/transit/demand?from=F1",             "Demand from an origin node");
        endpoints.put("GET /api/transit/demand?to=3",                "Demand to a destination node");

        info.put("endpoints", endpoints);
        return info;
    }
}