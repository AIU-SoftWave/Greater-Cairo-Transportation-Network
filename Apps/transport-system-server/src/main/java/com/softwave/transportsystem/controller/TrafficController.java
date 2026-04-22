package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.TrafficPattern;
import com.softwave.transportsystem.service.TrafficService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * REST controller for time-of-day traffic pattern data.
 *
 * Base path: /api/traffic
 *
 * Endpoints:
 *   GET  /api/traffic                              – all traffic patterns
 *   GET  /api/traffic/{roadId}                     – pattern for one road (e.g. "1-3")
 *   GET  /api/traffic/morning-congestion?minVph=2500 – congested roads in AM peak
 *   GET  /api/traffic/evening-congestion?minVph=2500 – congested roads in PM peak
 */
@RestController
@RequestMapping("/api/traffic")
public class TrafficController {

    private final TrafficService trafficService;

    public TrafficController(TrafficService trafficService) {
        this.trafficService = trafficService;
    }

    @GetMapping
    public List<TrafficPattern> getAll() {
        return trafficService.getAll();
    }

    @GetMapping("/{roadId}")
    public ResponseEntity<TrafficPattern> getByRoadId(@PathVariable String roadId) {
        return trafficService.getByRoadId(roadId)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/morning-congestion")
    public List<TrafficPattern> getMorningCongestion(
            @RequestParam(defaultValue = "2500") int minVph) {

        return trafficService.getMorningCongestion(minVph);
    }

    @GetMapping("/evening-congestion")
    public List<TrafficPattern> getEveningCongestion(
            @RequestParam(defaultValue = "2500") int minVph) {

        return trafficService.getEveningCongestion(minVph);
    }
}
