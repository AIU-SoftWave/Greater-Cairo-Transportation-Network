package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.Neighborhood;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

/**
 * Business logic for neighborhoods/districts.
 *
 * Provides:
 *  - Listing all neighborhoods
 *  - Finding a single neighborhood by its integer ID
 *  - Filtering neighborhoods by land-use type (Residential, Mixed, etc.)
 */
@Service
public class NeighborhoodService {

    private final DataLoaderService data;

    public NeighborhoodService(DataLoaderService data) {
        this.data = data;
    }

    /** Returns every neighborhood in the dataset. */
    public List<Neighborhood> getAll() {
        return data.getNeighborhoods();
    }

    /** Returns the neighborhood with the given ID, or empty if not found. */
    public Optional<Neighborhood> getById(int id) {
        return data.getNeighborhoods().stream()
            .filter(n -> n.getId() == id)
            .findFirst();
    }

    /**
     * Returns all neighborhoods whose type matches the given string
     * (case-insensitive).  E.g. getByType("residential").
     */
    public List<Neighborhood> getByType(String type) {
        return data.getNeighborhoods().stream()
            .filter(n -> n.getType().equalsIgnoreCase(type))
            .toList();
    }

    /**
     * Returns neighborhoods sorted by population (highest first).
     * Useful for MST algorithms that prioritise high-density areas.
     */
    public List<Neighborhood> getByPopulationDesc() {
        return data.getNeighborhoods().stream()
            .sorted((a, b) -> Integer.compare(b.getPopulation(), a.getPopulation()))
            .toList();
    }
}
