package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.PotentialRoad;
import com.softwave.transportsystem.model.Road;
import com.softwave.transportsystem.service.RoadService;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/roads")
public class RoadController {

    private final RoadService roadService;

    public RoadController(RoadService roadService) {
        this.roadService = roadService;
    }

    @GetMapping
    public List<Road> findExisting(@RequestParam(required = false) String node) {
        return roadService.findExisting(node, null);
    }

    @GetMapping("/poor-condition")
    public List<Road> findPoorCondition(@RequestParam(defaultValue = "5") int maxCondition) {
        return roadService.findExisting(null, maxCondition);
    }

    @GetMapping("/potential")
    public List<PotentialRoad> findPotential() {
        return roadService.findPotential(false);
    }

    @GetMapping("/potential/by-cost")
    public List<PotentialRoad> findPotentialByCost() {
        return roadService.findPotential(true);
    }
}
