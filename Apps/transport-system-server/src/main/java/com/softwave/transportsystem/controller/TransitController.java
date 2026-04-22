package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.BusRoute;
import com.softwave.transportsystem.model.MetroLine;
import com.softwave.transportsystem.model.TransitDemand;
import com.softwave.transportsystem.service.TransitService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/transit")
public class TransitController {

    private final TransitService transitService;

    public TransitController(TransitService transitService) {
        this.transitService = transitService;
    }

    @GetMapping("/metro")
    public List<MetroLine> findMetroLines() {
        return transitService.findMetroLines();
    }

    @GetMapping("/metro/{lineId}")
    public ResponseEntity<MetroLine> findMetroLineById(@PathVariable String lineId) {
        return transitService.findMetroLineById(lineId)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/bus")
    public List<BusRoute> findBusRoutes(@RequestParam(required = false) String node) {
        return transitService.findBusRoutes(node, false);
    }

    @GetMapping("/bus/top-ridership")
    public List<BusRoute> findTopBusRoutes() {
        return transitService.findBusRoutes(null, true);
    }

    @GetMapping("/bus/{routeId}")
    public ResponseEntity<BusRoute> findBusRouteById(@PathVariable String routeId) {
        return transitService.findBusRouteById(routeId)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/demand")
    public List<TransitDemand> findDemand(
            @RequestParam(required = false) String from,
            @RequestParam(required = false) String to) {
        return transitService.findDemand(from, to);
    }
}
