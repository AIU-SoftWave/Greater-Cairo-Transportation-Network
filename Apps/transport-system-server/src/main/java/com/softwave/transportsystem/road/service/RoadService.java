package com.softwave.transportsystem.road.service;

import com.softwave.transportsystem.road.model.PotentialRoad;
import com.softwave.transportsystem.road.model.Road;
import com.softwave.transportsystem.road.repository.PotentialRoadRepository;
import com.softwave.transportsystem.road.repository.RoadRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class RoadService {

    private final RoadRepository roadRepository;
    private final PotentialRoadRepository potentialRoadRepository;

    public RoadService(RoadRepository roadRepository,
            PotentialRoadRepository potentialRoadRepository) {
        this.roadRepository = roadRepository;
        this.potentialRoadRepository = potentialRoadRepository;
    }

    public List<Road> findAllRoads() {
        return roadRepository.findAll();
    }

    public Optional<Road> findRoadById(Long id) {
        return roadRepository.findById(id);
    }

    public List<PotentialRoad> findAllPotentialRoads() {
        return potentialRoadRepository.findAll();
    }

    public Optional<PotentialRoad> findPotentialRoadById(Long id) {
        return potentialRoadRepository.findById(id);
    }
}
