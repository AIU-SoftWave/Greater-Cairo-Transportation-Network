package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.BusRoute;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface BusRouteRepository extends JpaRepository<BusRoute, String> {

    List<BusRoute> findAllByOrderByDailyPassengersDesc();

    @Query("select distinct route from BusRoute route join route.stops stop where lower(stop.nodeId) = lower(:nodeId)")
    List<BusRoute> findAllServingNode(@Param("nodeId") String nodeId);
}
