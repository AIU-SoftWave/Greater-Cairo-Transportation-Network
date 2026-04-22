package com.softwave.transportsystem.neighborhood.service;

import com.softwave.transportsystem.neighborhood.model.Neighborhood;
import com.softwave.transportsystem.neighborhood.repository.NeighborhoodRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class NeighborhoodService {

    private final NeighborhoodRepository neighborhoodRepository;

    public NeighborhoodService(NeighborhoodRepository neighborhoodRepository) {
        this.neighborhoodRepository = neighborhoodRepository;
    }

    public List<Neighborhood> findAll() {
        return neighborhoodRepository.findAll();
    }

    public Optional<Neighborhood> findById(int id) {
        return neighborhoodRepository.findByNodeIdIgnoreCase(String.valueOf(id));
    }
}
