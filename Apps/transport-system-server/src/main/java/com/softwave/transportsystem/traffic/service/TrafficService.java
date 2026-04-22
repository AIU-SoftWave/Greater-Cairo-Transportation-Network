package com.softwave.transportsystem.traffic.service;

import com.softwave.transportsystem.traffic.model.TrafficPattern;
import com.softwave.transportsystem.traffic.repository.TrafficPatternRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class TrafficService {

    private final TrafficPatternRepository trafficPatternRepository;

    public TrafficService(TrafficPatternRepository trafficPatternRepository) {
        this.trafficPatternRepository = trafficPatternRepository;
    }

    public List<TrafficPattern> findAll() {
        return trafficPatternRepository.findAll();
    }

    public Optional<TrafficPattern> findById(Long id) {
        return trafficPatternRepository.findById(id);
    }
}
