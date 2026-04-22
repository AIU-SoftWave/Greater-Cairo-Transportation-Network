package com.softwave.transportsystem.transit.repository;

import com.softwave.transportsystem.transit.model.TransitDemand;
import org.springframework.data.jpa.repository.JpaRepository;

public interface TransitDemandRepository extends JpaRepository<TransitDemand, Long> {
}
