package com.softwave.transportsystem.transit.controller;

import com.softwave.transportsystem.transit.model.BusRoute;
import com.softwave.transportsystem.transit.service.BusService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/bus")
public class BusController {

    private final BusService busService;

    public BusController(BusService busService) {
        this.busService = busService;
    }

    @GetMapping
    public List<BusRoute> findAll() {
        return busService.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<BusRoute> findById(@PathVariable String id) {
        return busService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
