package com.softwave.transportsystem.transit.controller;

import com.softwave.transportsystem.transit.model.MetroLine;
import com.softwave.transportsystem.transit.service.MetroService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/metro")
public class MetroController {

    private final MetroService metroService;

    public MetroController(MetroService metroService) {
        this.metroService = metroService;
    }

    @GetMapping
    public List<MetroLine> findAll() {
        return metroService.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<MetroLine> findById(@PathVariable String id) {
        return metroService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }
}
