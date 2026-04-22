package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.PotentialRoad;
import com.softwave.transportsystem.model.Road;
import org.springframework.stereotype.Service;

import java.util.List;

/**
 * Business logic for road segments (both existing and potential).
 *
 * Provides:
 *  - Listing all existing roads
 *  - Listing all potential (new) roads
 *  - Filtering roads by condition score
 *  - Finding all roads connected to a given node
 */
@Service
public class RoadService {

    private final DataLoaderService data;

    public RoadService(DataLoaderService data) {
        this.data = data;
    }

    /** Returns all existing road segments. */
    public List<Road> getAllExisting() {
        return data.getRoads();
    }

    /** Returns all proposed (not yet built) road segments. */
    public List<PotentialRoad> getAllPotential() {
        return data.getPotentialRoads();
    }

    /**
     * Returns existing roads whose condition score is at or below the given
     * threshold.  A low score (1-4) indicates roads that need maintenance.
     */
    public List<Road> getPoorConditionRoads(int maxCondition) {
        return data.getRoads().stream()
            .filter(r -> r.getCondition() <= maxCondition)
            .toList();
    }

    /**
     * Returns all existing roads that start OR end at the given node ID.
     * Useful for building adjacency lists for graph algorithms.
     */
    public List<Road> getRoadsForNode(String nodeId) {
        return data.getRoads().stream()
            .filter(r -> r.connects(nodeId))
            .toList();
    }

    /**
     * Returns potential roads sorted by construction cost ascending.
     * Useful for greedy / MST approaches that minimise total spending.
     */
    public List<PotentialRoad> getPotentialSortedByCost() {
        return data.getPotentialRoads().stream()
            .sorted((a, b) -> Integer.compare(a.getConstructionCostMEgp(),
                                              b.getConstructionCostMEgp()))
            .toList();
    }
}
