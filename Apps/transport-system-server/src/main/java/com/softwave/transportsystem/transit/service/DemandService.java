package com.softwave.transportsystem.transit.service;

import com.softwave.transportsystem.transit.model.TransitDemand;
import com.softwave.transportsystem.transit.repository.TransitDemandRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class DemandService {

    private final TransitDemandRepository transitDemandRepository;

    public DemandService(TransitDemandRepository transitDemandRepository) {
        this.transitDemandRepository = transitDemandRepository;
    }

    public List<TransitDemand> findAll() {
        return transitDemandRepository.findAll();
    }

    public Optional<TransitDemand> findById(Long id) {
        return transitDemandRepository.findById(id);
    }
}
