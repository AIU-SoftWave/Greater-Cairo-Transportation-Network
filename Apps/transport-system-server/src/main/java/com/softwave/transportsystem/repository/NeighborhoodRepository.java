package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.Neighborhood;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface NeighborhoodRepository extends JpaRepository<Neighborhood, String> {

    Optional<Neighborhood> findByNodeIdIgnoreCase(String nodeId);

    List<Neighborhood> findByTypeIgnoreCase(String type);

    List<Neighborhood> findAllByOrderByPopulationDesc();
}
