package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.Facility;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface FacilityRepository extends JpaRepository<Facility, String> {

    Optional<Facility> findByNodeIdIgnoreCase(String nodeId);
    
    List<Facility> findByTypeIgnoreCase(String type);
}
