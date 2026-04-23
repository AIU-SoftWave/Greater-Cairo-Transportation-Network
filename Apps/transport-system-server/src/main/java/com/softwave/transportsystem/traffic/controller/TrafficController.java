package com.softwave.transportsystem.traffic.controller;

import com.softwave.transportsystem.traffic.model.TrafficPattern;
import com.softwave.transportsystem.traffic.service.GreedySignalTimingService;
import com.softwave.transportsystem.traffic.service.TrafficService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/traffic")
public class TrafficController {

    private final TrafficService trafficService;
    private final GreedySignalTimingService greedySignalTimingService;

    public TrafficController(TrafficService trafficService,
            GreedySignalTimingService greedySignalTimingService) {
        this.trafficService = trafficService;
        this.greedySignalTimingService = greedySignalTimingService;
    }

    @GetMapping
    public List<TrafficPattern> findAll() {
        return trafficService.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<TrafficPattern> findById(@PathVariable Long id) {
        return trafficService.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    /**
     * [PLACEHOLDER] Greedy traffic-signal timing optimisation.
     *
     * <p>Returns a "not implemented" string until a team member implements
     * {@link GreedySignalTimingService#computeSignalTiming}.</p>
     *
     * @param timeSlot one of {@code MORNING}, {@code AFTERNOON},
     *                 {@code EVENING}, {@code NIGHT}
     * @return placeholder string
     */
    @GetMapping("/signal-timing")
    public ResponseEntity<String> signalTiming(@RequestParam String timeSlot) {
        return ResponseEntity.ok(greedySignalTimingService.computeSignalTiming(timeSlot));
    }
}
