package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.TrafficPattern;
import com.softwave.transportsystem.service.TrafficService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/traffic")
public class TrafficController {

    private final TrafficService trafficService;

    public TrafficController(TrafficService trafficService) {
        this.trafficService = trafficService;
    }

    @GetMapping
    public List<TrafficPattern> findAll() {
        return trafficService.findAll(null, null);
    }

    @GetMapping("/{roadId}")
    public ResponseEntity<TrafficPattern> findByRoadId(@PathVariable String roadId) {
        return trafficService.findByRoadId(roadId)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/morning-congestion")
    public List<TrafficPattern> findMorningCongestion(@RequestParam(defaultValue = "2500") int minVph) {
        return trafficService.findAll("morning", minVph);
    }

    @GetMapping("/evening-congestion")
    public List<TrafficPattern> findEveningCongestion(@RequestParam(defaultValue = "2500") int minVph) {
        return trafficService.findAll("evening", minVph);
    }
}
