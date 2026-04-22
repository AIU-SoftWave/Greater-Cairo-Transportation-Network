package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.Facility;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

/**
 * Business logic for important facilities (hospitals, airports, etc.).
 *
 * Provides:
 *  - Listing all facilities
 *  - Finding a facility by its string ID (e.g. "F9")
 *  - Filtering facilities by category type
 */
@Service
public class FacilityService {

    private final DataLoaderService data;

    public FacilityService(DataLoaderService data) {
        this.data = data;
    }

    /** Returns every facility. */
    public List<Facility> getAll() {
        return data.getFacilities();
    }

    /** Returns the facility with the given ID (e.g. "F1"), or empty. */
    public Optional<Facility> getById(String id) {
        return data.getFacilities().stream()
            .filter(f -> f.getId().equalsIgnoreCase(id))
            .findFirst();
    }

    /**
     * Returns facilities filtered by type (case-insensitive).
     * E.g. getByType("Medical") returns hospitals.
     */
    public List<Facility> getByType(String type) {
        return data.getFacilities().stream()
            .filter(f -> f.getType().equalsIgnoreCase(type))
            .toList();
    }
}
