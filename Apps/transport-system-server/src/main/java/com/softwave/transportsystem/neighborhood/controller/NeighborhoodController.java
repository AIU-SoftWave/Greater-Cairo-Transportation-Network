package com.softwave.transportsystem.neighborhood.controller;

import com.softwave.transportsystem.neighborhood.model.Neighborhood;
import com.softwave.transportsystem.neighborhood.service.NeighborhoodService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/neighborhoods")
public class NeighborhoodController {

    private final NeighborhoodService neighborhoodService;

    public NeighborhoodController(NeighborhoodService neighborhoodService) {
        this.neighborhoodService = neighborhoodService;
    }

    @GetMapping
    public List<Neighborhood> findAll() {
        return neighborhoodService.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Neighborhood> findById(@PathVariable int id) {
        return neighborhoodService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
