package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.TrafficPattern;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface TrafficPatternRepository extends JpaRepository<TrafficPattern, Long> {

    Optional<TrafficPattern> findByRoad_FromNode_NodeIdIgnoreCaseAndRoad_ToNode_NodeIdIgnoreCase(String fromNodeId, String toNodeId);

    List<TrafficPattern> findByMorningPeakVphGreaterThanEqualOrderByMorningPeakVphDesc(int minVph);

    List<TrafficPattern> findByEveningPeakVphGreaterThanEqualOrderByEveningPeakVphDesc(int minVph);
}
