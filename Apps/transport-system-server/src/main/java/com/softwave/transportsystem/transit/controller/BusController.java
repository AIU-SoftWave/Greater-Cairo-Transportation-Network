package com.softwave.transportsystem.transit.controller;

import com.softwave.transportsystem.transit.model.BusRoute;
import com.softwave.transportsystem.transit.service.BusService;
import com.softwave.transportsystem.transit.service.DpSchedulingService;
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
    private final DpSchedulingService dpSchedulingService;

    public BusController(BusService busService,
            DpSchedulingService dpSchedulingService) {
        this.busService = busService;
        this.dpSchedulingService = dpSchedulingService;
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

    /**
     * [PLACEHOLDER] DP bus-fleet scheduling optimisation.
     *
     * <p>Returns a "not implemented" string until a team member implements
     * {@link DpSchedulingService#optimizeBusFleet}.</p>
     *
     * @return placeholder string
     */
    @GetMapping("/fleet-optimisation")
    public ResponseEntity<String> fleetOptimisation() {
        return ResponseEntity.ok(dpSchedulingService.optimizeBusFleet());
    }
}
