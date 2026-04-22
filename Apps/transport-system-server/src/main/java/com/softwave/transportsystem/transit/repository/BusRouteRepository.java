package com.softwave.transportsystem.transit.repository;

import com.softwave.transportsystem.transit.model.BusRoute;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BusRouteRepository extends JpaRepository<BusRoute, String> {
}
