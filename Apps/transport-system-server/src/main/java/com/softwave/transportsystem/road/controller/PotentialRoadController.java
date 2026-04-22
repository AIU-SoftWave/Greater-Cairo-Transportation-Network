package com.softwave.transportsystem.road.controller;

import com.softwave.transportsystem.road.model.PotentialRoad;
import com.softwave.transportsystem.road.service.RoadService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/potential-roads")
public class PotentialRoadController {

    private final RoadService roadService;

    public PotentialRoadController(RoadService roadService) {
        this.roadService = roadService;
    }

    @GetMapping
    public List<PotentialRoad> findAll() {
        return roadService.findAllPotentialRoads();
    }

    @GetMapping("/{id}")
    public ResponseEntity<PotentialRoad> findById(@PathVariable Long id) {
        return roadService.findPotentialRoadById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
