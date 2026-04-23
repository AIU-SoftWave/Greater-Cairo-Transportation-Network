package com.softwave.transportsystem.transit.controller;

import com.softwave.transportsystem.transit.model.MetroLine;
import com.softwave.transportsystem.transit.service.DpSchedulingService;
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
    private final DpSchedulingService dpSchedulingService;

    public MetroController(MetroService metroService,
            DpSchedulingService dpSchedulingService) {
        this.metroService = metroService;
        this.dpSchedulingService = dpSchedulingService;
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

    /**
     * [PLACEHOLDER] DP metro frequency scheduling optimisation.
     *
     * <p>Returns a "not implemented" string until a team member implements
     * {@link DpSchedulingService#optimizeMetroFrequency}.</p>
     *
     * @return placeholder string
     */
    @GetMapping("/frequency-optimisation")
    public ResponseEntity<String> frequencyOptimisation() {
        return ResponseEntity.ok(dpSchedulingService.optimizeMetroFrequency());
    }
}
