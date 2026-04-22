package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.Neighborhood;
import com.softwave.transportsystem.service.NeighborhoodService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * REST controller for neighborhood / district data.
 *
 * Base path: /api/neighborhoods
 *
 * Endpoints:
 *   GET  /api/neighborhoods                    – list all 15 neighborhoods
 *   GET  /api/neighborhoods/{id}               – get one by numeric ID
 *   GET  /api/neighborhoods?type=Residential   – filter by land-use type
 *   GET  /api/neighborhoods/top-population     – sorted by population (desc)
 */
@RestController
@RequestMapping("/api/neighborhoods")
public class NeighborhoodController {

    private final NeighborhoodService neighborhoodService;

    public NeighborhoodController(NeighborhoodService neighborhoodService) {
        this.neighborhoodService = neighborhoodService;
    }

    @GetMapping
    public List<Neighborhood> getAll(
            @RequestParam(required = false) String type) {

        if (type != null && !type.isBlank()) {
            return neighborhoodService.getByType(type);
        }
        return neighborhoodService.getAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Neighborhood> getById(@PathVariable int id) {
        return neighborhoodService.getById(id)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/top-population")
    public List<Neighborhood> getTopByPopulation() {
        return neighborhoodService.getByPopulationDesc();
    }
}
