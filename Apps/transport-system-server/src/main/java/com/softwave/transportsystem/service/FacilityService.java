package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.Facility;
import com.softwave.transportsystem.repository.FacilityRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class FacilityService {

    private final FacilityRepository facilityRepository;

    public FacilityService(FacilityRepository facilityRepository) {
        this.facilityRepository = facilityRepository;
    }

    public List<Facility> findAll(String type) {
        if (type == null || type.isBlank()) {
            return facilityRepository.findAll();
        }
        return facilityRepository.findByTypeIgnoreCase(type);
    }

    public Optional<Facility> findById(String id) {
        return facilityRepository.findByNodeIdIgnoreCase(id);
    }
}
