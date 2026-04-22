package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.BusRoute;
import com.softwave.transportsystem.model.MetroLine;
import com.softwave.transportsystem.model.TransitDemand;
import com.softwave.transportsystem.service.TransitService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * REST controller for public transit data (metro, bus, demand).
 *
 * Base path: /api/transit
 *
 * Endpoints:
 *   GET  /api/transit/metro                      – all metro lines
 *   GET  /api/transit/metro/{lineId}             – one metro line (e.g. "M1")
 *   GET  /api/transit/bus                        – all bus routes
 *   GET  /api/transit/bus/{routeId}              – one bus route (e.g. "B3")
 *   GET  /api/transit/bus?node=3                 – bus routes that serve node "3"
 *   GET  /api/transit/bus/top-ridership          – bus routes sorted by passengers
 *   GET  /api/transit/demand                     – all OD demand records
 *   GET  /api/transit/demand?from=F1             – demand originating from a node
 *   GET  /api/transit/demand?to=3                – demand destined for a node
 */
@RestController
@RequestMapping("/api/transit")
public class TransitController {

    private final TransitService transitService;

    public TransitController(TransitService transitService) {
        this.transitService = transitService;
    }

    // ── Metro ────────────────────────────────────────────────────────────────

    @GetMapping("/metro")
    public List<MetroLine> getAllMetro() {
        return transitService.getAllMetroLines();
    }

    @GetMapping("/metro/{lineId}")
    public ResponseEntity<MetroLine> getMetroById(@PathVariable String lineId) {
        return transitService.getMetroLineById(lineId)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }

    // ── Bus ──────────────────────────────────────────────────────────────────

    @GetMapping("/bus")
    public List<BusRoute> getAllBus(
            @RequestParam(required = false) String node) {

        if (node != null && !node.isBlank()) {
            return transitService.getBusRoutesForNode(node);
        }
        return transitService.getAllBusRoutes();
    }

    @GetMapping("/bus/top-ridership")
    public List<BusRoute> getBusByRidership() {
        return transitService.getBusRoutesByRidershipDesc();
    }

    @GetMapping("/bus/{routeId}")
    public ResponseEntity<BusRoute> getBusById(@PathVariable String routeId) {
        return transitService.getBusRouteById(routeId)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }

    // ── Demand ───────────────────────────────────────────────────────────────

    @GetMapping("/demand")
    public List<TransitDemand> getDemand(
            @RequestParam(required = false) String from,
            @RequestParam(required = false) String to) {

        if (from != null && !from.isBlank()) {
            return transitService.getDemandFrom(from);
        }
        if (to != null && !to.isBlank()) {
            return transitService.getDemandTo(to);
        }
        return transitService.getAllDemand();
    }
}
