package com.softwave.transportsystem.controller;

import com.softwave.transportsystem.model.PotentialRoad;
import com.softwave.transportsystem.model.Road;
import com.softwave.transportsystem.service.RoadService;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * REST controller for road network data (existing and potential).
 *
 * Base path: /api/roads
 *
 * Endpoints:
 *   GET  /api/roads                                – list all existing roads
 *   GET  /api/roads?node=3                         – all roads touching node "3"
 *   GET  /api/roads/poor-condition?maxCondition=5  – roads needing maintenance
 *   GET  /api/roads/potential                      – all proposed new roads
 *   GET  /api/roads/potential/by-cost              – potential roads sorted cheapest first
 */
@RestController
@RequestMapping("/api/roads")
public class RoadController {

    private final RoadService roadService;

    public RoadController(RoadService roadService) {
        this.roadService = roadService;
    }

    @GetMapping
    public List<Road> getExisting(
            @RequestParam(required = false) String node) {

        if (node != null && !node.isBlank()) {
            return roadService.getRoadsForNode(node);
        }
        return roadService.getAllExisting();
    }

    @GetMapping("/poor-condition")
    public List<Road> getPoorCondition(
            @RequestParam(defaultValue = "5") int maxCondition) {

        return roadService.getPoorConditionRoads(maxCondition);
    }

    @GetMapping("/potential")
    public List<PotentialRoad> getPotential() {
        return roadService.getAllPotential();
    }

    @GetMapping("/potential/by-cost")
    public List<PotentialRoad> getPotentialByCost() {
        return roadService.getPotentialSortedByCost();
    }
}
