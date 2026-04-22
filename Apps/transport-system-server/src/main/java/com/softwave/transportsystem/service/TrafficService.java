package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.TrafficPattern;
import com.softwave.transportsystem.repository.TrafficPatternRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class TrafficService {

    private final TrafficPatternRepository trafficPatternRepository;

    public TrafficService(TrafficPatternRepository trafficPatternRepository) {
        this.trafficPatternRepository = trafficPatternRepository;
    }

    public List<TrafficPattern> findAll(String period, Integer minVph) {
        if ("morning".equalsIgnoreCase(period) && minVph != null) {
            return trafficPatternRepository.findByMorningPeakVphGreaterThanEqualOrderByMorningPeakVphDesc(minVph);
        }
        if ("evening".equalsIgnoreCase(period) && minVph != null) {
            return trafficPatternRepository.findByEveningPeakVphGreaterThanEqualOrderByEveningPeakVphDesc(minVph);
        }
        return trafficPatternRepository.findAll();
    }

    public Optional<TrafficPattern> findByRoadId(String roadId) {
        String[] parts = roadId.split("-", 2);
        if (parts.length != 2) {
            return Optional.empty();
        }
        return trafficPatternRepository.findByRoad_FromNode_NodeIdIgnoreCaseAndRoad_ToNode_NodeIdIgnoreCase(parts[0], parts[1]);
    }
}
