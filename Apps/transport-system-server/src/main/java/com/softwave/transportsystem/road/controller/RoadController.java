package com.softwave.transportsystem.road.controller;

import com.softwave.transportsystem.road.model.Road;
import com.softwave.transportsystem.road.service.RoadService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/roads")
public class RoadController {

    private final RoadService roadService;

    public RoadController(RoadService roadService) {
        this.roadService = roadService;
    }

    @GetMapping
    public List<Road> findAll() {
        return roadService.findAllRoads();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Road> findById(@PathVariable Long id) {
        return roadService.findRoadById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
