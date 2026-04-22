package com.softwave.transportsystem.facility.repository;

import com.softwave.transportsystem.facility.model.Facility;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface FacilityRepository extends JpaRepository<Facility, String> {

    Optional<Facility> findByNodeIdIgnoreCase(String nodeId);
}
