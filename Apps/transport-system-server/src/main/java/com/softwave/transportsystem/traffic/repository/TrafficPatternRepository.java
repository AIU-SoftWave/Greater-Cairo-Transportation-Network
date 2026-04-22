package com.softwave.transportsystem.traffic.repository;

import com.softwave.transportsystem.traffic.model.TrafficPattern;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface TrafficPatternRepository extends JpaRepository<TrafficPattern, Long> {

    Optional<TrafficPattern> findByRoad_FromNode_NodeIdIgnoreCaseAndRoad_ToNode_NodeIdIgnoreCase(
            String fromNodeId, String toNodeId);
}
