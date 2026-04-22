package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.Neighborhood;
import com.softwave.transportsystem.repository.NeighborhoodRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class NeighborhoodService {

    private final NeighborhoodRepository neighborhoodRepository;

    public NeighborhoodService(NeighborhoodRepository neighborhoodRepository) {
        this.neighborhoodRepository = neighborhoodRepository;
    }

    public List<Neighborhood> findAll(String type, boolean sortByPopulation) {
        if (sortByPopulation) {
            return neighborhoodRepository.findAllByOrderByPopulationDesc();
        }
        if (type == null || type.isBlank()) {
            return neighborhoodRepository.findAll();
        }
        return neighborhoodRepository.findByTypeIgnoreCase(type);
    }

    public Optional<Neighborhood> findById(int id) {
        return neighborhoodRepository.findByNodeIdIgnoreCase(String.valueOf(id));
    }
}
