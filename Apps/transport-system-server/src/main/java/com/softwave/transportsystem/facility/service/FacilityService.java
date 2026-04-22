package com.softwave.transportsystem.facility.service;

import com.softwave.transportsystem.facility.model.Facility;
import com.softwave.transportsystem.facility.repository.FacilityRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class FacilityService {

    private final FacilityRepository facilityRepository;

    public FacilityService(FacilityRepository facilityRepository) {
        this.facilityRepository = facilityRepository;
    }

    public List<Facility> findAll() {
        return facilityRepository.findAll();
    }

    public Optional<Facility> findById(String id) {
        return facilityRepository.findByNodeIdIgnoreCase(id);
    }
}
