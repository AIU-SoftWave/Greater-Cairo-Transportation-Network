package com.softwave.transportsystem.traffic.controller;

import com.softwave.transportsystem.traffic.model.TrafficPattern;
import com.softwave.transportsystem.traffic.service.TrafficService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
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
        return trafficService.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<TrafficPattern> findById(@PathVariable Long id) {
        return trafficService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
