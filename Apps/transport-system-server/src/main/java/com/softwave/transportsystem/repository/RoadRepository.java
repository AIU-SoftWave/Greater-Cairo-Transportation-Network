package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.Road;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface RoadRepository extends JpaRepository<Road, Long> {

    List<Road> findByConditionLessThanEqual(int maxCondition);
    
    List<Road> findByFromNode_NodeIdIgnoreCaseOrToNode_NodeIdIgnoreCase(String fromNodeId, String toNodeId);
}
