package com.softwave.transportsystem.transit.controller;

import com.softwave.transportsystem.transit.model.TransitDemand;
import com.softwave.transportsystem.transit.service.DemandService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/demand")
public class DemandController {

    private final DemandService demandService;

    public DemandController(DemandService demandService) {
        this.demandService = demandService;
    }

    @GetMapping
    public List<TransitDemand> findAll() {
        return demandService.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<TransitDemand> findById(@PathVariable Long id) {
        return demandService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
