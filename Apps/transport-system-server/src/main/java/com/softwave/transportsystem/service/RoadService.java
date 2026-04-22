package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.PotentialRoad;
import com.softwave.transportsystem.model.Road;
import com.softwave.transportsystem.repository.PotentialRoadRepository;
import com.softwave.transportsystem.repository.RoadRepository;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class RoadService {

    private final RoadRepository roadRepository;
    private final PotentialRoadRepository potentialRoadRepository;

    public RoadService(RoadRepository roadRepository,
            PotentialRoadRepository potentialRoadRepository) {
        this.roadRepository = roadRepository;
        this.potentialRoadRepository = potentialRoadRepository;
    }

    public List<Road> findExisting(String nodeId, Integer maxCondition) {
        if (maxCondition != null) {
            return roadRepository.findByConditionLessThanEqual(maxCondition);
        }
        if (nodeId == null || nodeId.isBlank()) {
            return roadRepository.findAll();
        }
        return roadRepository.findByFromNode_NodeIdIgnoreCaseOrToNode_NodeIdIgnoreCase(nodeId, nodeId);
    }

    public List<PotentialRoad> findPotential(boolean sortByCost) {
        if (sortByCost) {
            return potentialRoadRepository.findAllByOrderByConstructionCostMEgpAsc();
        }
        return potentialRoadRepository.findAll();
    }
}
