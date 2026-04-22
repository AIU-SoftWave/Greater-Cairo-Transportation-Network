package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.TransitDemand;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface TransitDemandRepository extends JpaRepository<TransitDemand, Long> {

    List<TransitDemand> findByFromNode_NodeIdIgnoreCaseOrderByDailyPassengersDesc(String fromNodeId);

    List<TransitDemand> findByToNode_NodeIdIgnoreCaseOrderByDailyPassengersDesc(String toNodeId);
}
