package com.softwave.transportsystem.neighborhood.repository;

import com.softwave.transportsystem.neighborhood.model.Neighborhood;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface NeighborhoodRepository extends JpaRepository<Neighborhood, String> {

    Optional<Neighborhood> findByNodeIdIgnoreCase(String nodeId);
}
