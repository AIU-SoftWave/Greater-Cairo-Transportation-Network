package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.PotentialRoad;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface PotentialRoadRepository extends JpaRepository<PotentialRoad, Long> {

    List<PotentialRoad> findAllByOrderByConstructionCostMEgpAsc();
}
