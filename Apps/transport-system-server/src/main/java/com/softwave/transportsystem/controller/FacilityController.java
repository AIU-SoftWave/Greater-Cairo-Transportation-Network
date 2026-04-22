package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.Facility;
import com.softwave.transportsystem.service.FacilityService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/facilities")
public class FacilityController {

    private final FacilityService facilityService;

    public FacilityController(FacilityService facilityService) {
        this.facilityService = facilityService;
    }

    @GetMapping
    public List<Facility> findAll(@RequestParam(required = false) String type) {
        return facilityService.findAll(type);
    }

    @GetMapping("/{id}")
    public ResponseEntity<Facility> findById(@PathVariable String id) {
        return facilityService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
