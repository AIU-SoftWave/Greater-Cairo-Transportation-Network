package com.softwave.transportsystem.transit.service;

import com.softwave.transportsystem.transit.model.BusRoute;
import com.softwave.transportsystem.transit.repository.BusRouteRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class BusService {

    private final BusRouteRepository busRouteRepository;

    public BusService(BusRouteRepository busRouteRepository) {
        this.busRouteRepository = busRouteRepository;
    }

    public List<BusRoute> findAll() {
        return busRouteRepository.findAll();
    }

    public Optional<BusRoute> findById(String routeId) {
        return busRouteRepository.findById(routeId);
    }
}
