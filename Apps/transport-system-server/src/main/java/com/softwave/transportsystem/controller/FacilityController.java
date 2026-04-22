package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.Facility;
import com.softwave.transportsystem.service.FacilityService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * REST controller for important facility data.
 *
 * Base path: /api/facilities
 *
 * Endpoints:
 *   GET  /api/facilities               – list all facilities
 *   GET  /api/facilities/{id}          – get one by ID (e.g. "F9")
 *   GET  /api/facilities?type=Medical  – filter by facility type
 */
@RestController
@RequestMapping("/api/facilities")
public class FacilityController {

    private final FacilityService facilityService;

    public FacilityController(FacilityService facilityService) {
        this.facilityService = facilityService;
    }

    @GetMapping
    public List<Facility> getAll(
            @RequestParam(required = false) String type) {

        if (type != null && !type.isBlank()) {
            return facilityService.getByType(type);
        }
        return facilityService.getAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Facility> getById(@PathVariable String id) {
        return facilityService.getById(id)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }
}
