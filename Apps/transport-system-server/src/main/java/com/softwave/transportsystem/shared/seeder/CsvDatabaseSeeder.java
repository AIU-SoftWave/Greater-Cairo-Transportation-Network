package com.softwave.transportsystem.shared.seeder;

import com.softwave.transportsystem.facility.model.Facility;
import com.softwave.transportsystem.facility.repository.FacilityRepository;
import com.softwave.transportsystem.neighborhood.model.Neighborhood;
import com.softwave.transportsystem.neighborhood.repository.NeighborhoodRepository;
import com.softwave.transportsystem.road.model.PotentialRoad;
import com.softwave.transportsystem.road.model.Road;
import com.softwave.transportsystem.road.repository.PotentialRoadRepository;
import com.softwave.transportsystem.road.repository.RoadRepository;
import com.softwave.transportsystem.shared.model.AbstractNode;
import com.softwave.transportsystem.shared.repository.NodeRepository;
import com.softwave.transportsystem.traffic.model.TrafficPattern;
import com.softwave.transportsystem.traffic.repository.TrafficPatternRepository;
import com.softwave.transportsystem.transit.model.BusRoute;
import com.softwave.transportsystem.transit.model.MetroLine;
import com.softwave.transportsystem.transit.model.TransitDemand;
import com.softwave.transportsystem.transit.repository.BusRouteRepository;
import com.softwave.transportsystem.transit.repository.MetroLineRepository;
import com.softwave.transportsystem.transit.repository.TransitDemandRepository;
import org.apache.commons.csv.CSVFormat;
import org.apache.commons.csv.CSVParser;
import org.apache.commons.csv.CSVRecord;
import org.springframework.boot.CommandLineRunner;
import org.springframework.core.io.ClassPathResource;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/**
 * Seeds the database from the CSV files once, then leaves the database as the
 * application's source of truth.
 */
@Component
public class CsvDatabaseSeeder implements CommandLineRunner {

    private final NodeRepository nodeRepository;
    private final NeighborhoodRepository neighborhoodRepository;
    private final FacilityRepository facilityRepository;
    private final RoadRepository roadRepository;
    private final PotentialRoadRepository potentialRoadRepository;
    private final TrafficPatternRepository trafficPatternRepository;
    private final MetroLineRepository metroLineRepository;
    private final BusRouteRepository busRouteRepository;
    private final TransitDemandRepository transitDemandRepository;

    public CsvDatabaseSeeder(NodeRepository nodeRepository,
            NeighborhoodRepository neighborhoodRepository,
            FacilityRepository facilityRepository,
            RoadRepository roadRepository,
            PotentialRoadRepository potentialRoadRepository,
            TrafficPatternRepository trafficPatternRepository,
            MetroLineRepository metroLineRepository,
            BusRouteRepository busRouteRepository,
            TransitDemandRepository transitDemandRepository) {
        this.nodeRepository = nodeRepository;
        this.neighborhoodRepository = neighborhoodRepository;
        this.facilityRepository = facilityRepository;
        this.roadRepository = roadRepository;
        this.potentialRoadRepository = potentialRoadRepository;
        this.trafficPatternRepository = trafficPatternRepository;
        this.metroLineRepository = metroLineRepository;
        this.busRouteRepository = busRouteRepository;
        this.transitDemandRepository = transitDemandRepository;
    }

    @Override
    @Transactional
    public void run(String... args) {
        if (nodeRepository.count() > 0) {
            return;
        }

        Map<String, AbstractNode> nodesById = seedNodes();
        Map<String, Road> roadsById = seedRoads(nodesById);
        seedPotentialRoads(nodesById);
        seedTrafficPatterns(roadsById);
        seedMetroLines(nodesById);
        seedBusRoutes(nodesById);
        seedTransitDemand(nodesById);
    }

    private Map<String, AbstractNode> seedNodes() {
        List<Neighborhood> neighborhoods = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/nodes.csv")) {
            neighborhoods.add(new Neighborhood(
                    Integer.parseInt(record.get("ID")),
                    record.get("Name"),
                    Integer.parseInt(record.get("Population")),
                    record.get("Type"),
                    Double.parseDouble(record.get("Longitude")),
                    Double.parseDouble(record.get("Latitude"))
            ));
        }
        neighborhoodRepository.saveAll(neighborhoods);

        List<Facility> facilities = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/facilities.csv")) {
            facilities.add(new Facility(
                    record.get("ID"),
                    record.get("Name"),
                    record.get("Type"),
                    Double.parseDouble(record.get("Longitude")),
                    Double.parseDouble(record.get("Latitude"))
            ));
        }
        facilityRepository.saveAll(facilities);

        Map<String, AbstractNode> nodesById = new LinkedHashMap<>();
        nodeRepository.findAll().forEach(node -> nodesById.put(node.getNodeId(), node));
        return nodesById;
    }

    private Map<String, Road> seedRoads(Map<String, AbstractNode> nodesById) {
        List<Road> roads = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/existing_roads.csv")) {
            roads.add(new Road(
                    resolveNode(nodesById, record.get("FromID")),
                    resolveNode(nodesById, record.get("ToID")),
                    Double.parseDouble(record.get("Distance_km")),
                    Integer.parseInt(record.get("Capacity_vph")),
                    Integer.parseInt(record.get("Condition"))
            ));
        }
        roadRepository.saveAll(roads);

        Map<String, Road> roadsById = new LinkedHashMap<>();
        roadRepository.findAll().forEach(road -> roadsById.put(road.asRoadId(), road));
        return roadsById;
    }

    private void seedPotentialRoads(Map<String, AbstractNode> nodesById) {
        List<PotentialRoad> potentialRoads = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/potential_roads.csv")) {
            potentialRoads.add(new PotentialRoad(
                    resolveNode(nodesById, record.get("FromID")),
                    resolveNode(nodesById, record.get("ToID")),
                    Double.parseDouble(record.get("Distance_km")),
                    Integer.parseInt(record.get("Capacity_vph")),
                    Integer.parseInt(record.get("Construction_Cost_Million_EGP"))
            ));
        }
        potentialRoadRepository.saveAll(potentialRoads);
    }

    private void seedTrafficPatterns(Map<String, Road> roadsById) {
        List<TrafficPattern> trafficPatterns = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/traffic_patterns.csv")) {
            trafficPatterns.add(new TrafficPattern(
                    resolveRoad(roadsById, record.get("RoadID")),
                    Integer.parseInt(record.get("Morning_Peak_vph")),
                    Integer.parseInt(record.get("Afternoon_vph")),
                    Integer.parseInt(record.get("Evening_Peak_vph")),
                    Integer.parseInt(record.get("Night_vph"))
            ));
        }
        trafficPatternRepository.saveAll(trafficPatterns);
    }

    private void seedMetroLines(Map<String, AbstractNode> nodesById) {
        List<MetroLine> metroLines = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/metro_lines.csv")) {
            metroLines.add(new MetroLine(
                    record.get("LineID"),
                    record.get("Name"),
                    resolveNodes(nodesById, record.get("Stations")),
                    Integer.parseInt(record.get("Daily_Passengers"))
            ));
        }
        metroLineRepository.saveAll(metroLines);
    }

    private void seedBusRoutes(Map<String, AbstractNode> nodesById) {
        List<BusRoute> busRoutes = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/bus_routes.csv")) {
            busRoutes.add(new BusRoute(
                    record.get("RouteID"),
                    resolveNodes(nodesById, record.get("Stops")),
                    Integer.parseInt(record.get("Buses_Assigned")),
                    Integer.parseInt(record.get("Daily_Passengers"))
            ));
        }
        busRouteRepository.saveAll(busRoutes);
    }

    private void seedTransitDemand(Map<String, AbstractNode> nodesById) {
        List<TransitDemand> transitDemands = new ArrayList<>();
        for (CSVRecord record : readCsv("static/data/transit_demand.csv")) {
            transitDemands.add(new TransitDemand(
                    resolveNode(nodesById, record.get("FromID")),
                    resolveNode(nodesById, record.get("ToID")),
                    Integer.parseInt(record.get("Daily_Passengers"))
            ));
        }
        transitDemandRepository.saveAll(transitDemands);
    }

    private Iterable<CSVRecord> readCsv(String classpathPath) {
        try {
            Reader reader = new InputStreamReader(
                    new ClassPathResource(classpathPath).getInputStream(),
                    StandardCharsets.UTF_8
            );
            CSVFormat format = CSVFormat.DEFAULT.builder()
                    .setHeader()
                    .setSkipHeaderRecord(true)
                    .setIgnoreEmptyLines(true)
                    .setTrim(true)
                    .build();
            return new CSVParser(reader, format);
        } catch (IOException exception) {
            throw new IllegalStateException("Failed to read CSV: " + classpathPath, exception);
        }
    }

    private List<AbstractNode> resolveNodes(Map<String, AbstractNode> nodesById, String rawNodeIds) {
        List<AbstractNode> nodes = new ArrayList<>();
        for (String nodeId : rawNodeIds.split(",")) {
            String trimmedNodeId = nodeId.trim();
            if (!trimmedNodeId.isEmpty()) {
                nodes.add(resolveNode(nodesById, trimmedNodeId));
            }
        }
        return nodes;
    }

    private AbstractNode resolveNode(Map<String, AbstractNode> nodesById, String nodeId) {
        AbstractNode node = nodesById.get(nodeId);
        if (node == null) {
            throw new IllegalArgumentException("Unknown node ID in seed data: " + nodeId);
        }
        return node;
    }

    private Road resolveRoad(Map<String, Road> roadsById, String roadId) {
        Road road = roadsById.get(roadId);
        if (road == null) {
            throw new IllegalArgumentException("Unknown road ID in seed data: " + roadId);
        }
        return road;
    }
}
