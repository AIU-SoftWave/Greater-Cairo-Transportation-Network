package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.BusRoute;
import com.softwave.transportsystem.model.MetroLine;
import com.softwave.transportsystem.model.TransitDemand;
import com.softwave.transportsystem.repository.BusRouteRepository;
import com.softwave.transportsystem.repository.MetroLineRepository;
import com.softwave.transportsystem.repository.TransitDemandRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class TransitService {

    private final MetroLineRepository metroLineRepository;
    private final BusRouteRepository busRouteRepository;
    private final TransitDemandRepository transitDemandRepository;

    public TransitService(MetroLineRepository metroLineRepository,
            BusRouteRepository busRouteRepository,
            TransitDemandRepository transitDemandRepository) {
        this.metroLineRepository = metroLineRepository;
        this.busRouteRepository = busRouteRepository;
        this.transitDemandRepository = transitDemandRepository;
    }

    public List<MetroLine> findMetroLines() {
        return metroLineRepository.findAll();
    }

    public Optional<MetroLine> findMetroLineById(String lineId) {
        return metroLineRepository.findById(lineId);
    }

    public List<BusRoute> findBusRoutes(String nodeId, boolean sortByRidership) {
        if (sortByRidership) {
            return busRouteRepository.findAllByOrderByDailyPassengersDesc();
        }
        if (nodeId == null || nodeId.isBlank()) {
            return busRouteRepository.findAll();
        }
        return busRouteRepository.findAllServingNode(nodeId);
    }

    public Optional<BusRoute> findBusRouteById(String routeId) {
        return busRouteRepository.findById(routeId);
    }

    public List<TransitDemand> findDemand(String fromId, String toId) {
        if (fromId != null && !fromId.isBlank()) {
            return transitDemandRepository.findByFromNode_NodeIdIgnoreCaseOrderByDailyPassengersDesc(fromId);
        }
        if (toId != null && !toId.isBlank()) {
            return transitDemandRepository.findByToNode_NodeIdIgnoreCaseOrderByDailyPassengersDesc(toId);
        }
        return transitDemandRepository.findAll();
    }
}
