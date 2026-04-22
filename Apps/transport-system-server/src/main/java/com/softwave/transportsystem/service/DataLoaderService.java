package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.*;
import com.softwave.transportsystem.model.Interfaces.AbstractNode;
import org.apache.commons.csv.CSVFormat;
import org.apache.commons.csv.CSVParser;
import org.apache.commons.csv.CSVRecord;
import org.springframework.stereotype.Service;

import jakarta.annotation.PostConstruct;
import java.io.InputStreamReader;
import java.io.Reader;
import java.nio.charset.StandardCharsets;
import java.util.*;

/**
 * Reads every CSV file from the classpath (resources/static/data/) once on
 * startup and keeps the parsed data in memory.
 *
 * All other services depend on this class as their single source of truth.
 * There is no database – the CSV files are the "database".
 *
 * Load order (PostConstruct): neighborhoods → facilities → roads →
 *   potentialRoads → trafficPatterns → metroLines → busRoutes → transitDemands
 */
@Service
public class DataLoaderService {

    // Each dataset is stored as an unmodifiable list after loading
    private List<Neighborhood>   neighborhoods   = new ArrayList<>();
    private List<Facility>       facilities      = new ArrayList<>();
    private List<Road>           roads           = new ArrayList<>();
    private List<PotentialRoad>  potentialRoads  = new ArrayList<>();
    private List<TrafficPattern> trafficPatterns = new ArrayList<>();
    private List<MetroLine>      metroLines      = new ArrayList<>();
    private List<BusRoute>       busRoutes       = new ArrayList<>();
    private List<TransitDemand>  transitDemands  = new ArrayList<>();
    private Map<String, AbstractNode> nodesById   = new LinkedHashMap<>();
    private Map<String, Road> roadsById           = new LinkedHashMap<>();

    /**
     * Called automatically by Spring after the bean is created.
     * Loads every CSV file and populates the in-memory lists.
     */
    @PostConstruct
    public void loadAll() {
        neighborhoods   = loadNeighborhoods();
        facilities      = loadFacilities();
        nodesById       = buildNodeIndex(neighborhoods, facilities);
        roads           = loadRoads();
        roadsById       = buildRoadIndex(roads);
        potentialRoads  = loadPotentialRoads();
        trafficPatterns = loadTrafficPatterns();
        metroLines      = loadMetroLines();
        busRoutes       = loadBusRoutes();
        transitDemands  = loadTransitDemands();
    }

    // ── Public accessors (read-only views) ────────────────────────────────────

    public List<Neighborhood>   getNeighborhoods()   { return Collections.unmodifiableList(neighborhoods); }
    public List<Facility>       getFacilities()      { return Collections.unmodifiableList(facilities); }
    public List<Road>           getRoads()           { return Collections.unmodifiableList(roads); }
    public List<PotentialRoad>  getPotentialRoads()  { return Collections.unmodifiableList(potentialRoads); }
    public List<TrafficPattern> getTrafficPatterns() { return Collections.unmodifiableList(trafficPatterns); }
    public List<MetroLine>      getMetroLines()      { return Collections.unmodifiableList(metroLines); }
    public List<BusRoute>       getBusRoutes()       { return Collections.unmodifiableList(busRoutes); }
    public List<TransitDemand>  getTransitDemands()  { return Collections.unmodifiableList(transitDemands); }
    public Collection<AbstractNode> getAllNodes()    { return Collections.unmodifiableCollection(nodesById.values()); }

    // ── Private CSV loaders ───────────────────────────────────────────────────

    private List<Neighborhood> loadNeighborhoods() {
        List<Neighborhood> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/nodes.csv")) {
            list.add(new Neighborhood(
                Integer.parseInt(r.get("ID")),
                r.get("Name"),
                Integer.parseInt(r.get("Population")),
                r.get("Type"),
                Double.parseDouble(r.get("Longitude")),
                Double.parseDouble(r.get("Latitude"))
            ));
        }
        return list;
    }

    private List<Facility> loadFacilities() {
        List<Facility> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/facilities.csv")) {
            list.add(new Facility(
                r.get("ID"),
                r.get("Name"),
                r.get("Type"),
                Double.parseDouble(r.get("Longitude")),
                Double.parseDouble(r.get("Latitude"))
            ));
        }
        return list;
    }

    private List<Road> loadRoads() {
        List<Road> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/existing_roads.csv")) {
            list.add(new Road(
                resolveNode(r.get("FromID")),
                resolveNode(r.get("ToID")),
                Double.parseDouble(r.get("Distance_km")),
                Integer.parseInt(r.get("Capacity_vph")),
                Integer.parseInt(r.get("Condition"))
            ));
        }
        return list;
    }

    private List<PotentialRoad> loadPotentialRoads() {
        List<PotentialRoad> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/potential_roads.csv")) {
            list.add(new PotentialRoad(
                resolveNode(r.get("FromID")),
                resolveNode(r.get("ToID")),
                Double.parseDouble(r.get("Distance_km")),
                Integer.parseInt(r.get("Capacity_vph")),
                Integer.parseInt(r.get("Construction_Cost_Million_EGP"))
            ));
        }
        return list;
    }

    private List<TrafficPattern> loadTrafficPatterns() {
        List<TrafficPattern> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/traffic_patterns.csv")) {
            String roadId = r.get("RoadID");
            list.add(new TrafficPattern(
                resolveRoad(roadId),
                Integer.parseInt(r.get("Morning_Peak_vph")),
                Integer.parseInt(r.get("Afternoon_vph")),
                Integer.parseInt(r.get("Evening_Peak_vph")),
                Integer.parseInt(r.get("Night_vph"))
            ));
        }
        return list;
    }

    private List<MetroLine> loadMetroLines() {
        List<MetroLine> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/metro_lines.csv")) {
            List<AbstractNode> stations = resolveNodes(splitNodeIds(r.get("Stations")));
            list.add(new MetroLine(
                r.get("LineID"),
                r.get("Name"),
                stations,
                Integer.parseInt(r.get("Daily_Passengers"))
            ));
        }
        return list;
    }

    private List<BusRoute> loadBusRoutes() {
        List<BusRoute> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/bus_routes.csv")) {
            List<AbstractNode> stops = resolveNodes(splitNodeIds(r.get("Stops")));
            list.add(new BusRoute(
                r.get("RouteID"),
                stops,
                Integer.parseInt(r.get("Buses_Assigned")),
                Integer.parseInt(r.get("Daily_Passengers"))
            ));
        }
        return list;
    }

    private List<TransitDemand> loadTransitDemands() {
        List<TransitDemand> list = new ArrayList<>();
        for (CSVRecord r : parseCsv("/static/data/transit_demand.csv")) {
            list.add(new TransitDemand(
                resolveNode(r.get("FromID")),
                resolveNode(r.get("ToID")),
                Integer.parseInt(r.get("Daily_Passengers"))
            ));
        }
        return list;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /**
     * Opens a classpath resource and returns a parsed CSVParser with headers.
     * Skips blank lines and trims surrounding whitespace from every value.
     */
    private Iterable<CSVRecord> parseCsv(String classpathPath) {
        try {
            Reader reader = new InputStreamReader(
                Objects.requireNonNull(
                    getClass().getResourceAsStream(classpathPath),
                    "CSV not found on classpath: " + classpathPath
                ),
                StandardCharsets.UTF_8
            );
            CSVFormat format = CSVFormat.DEFAULT.builder()
                .setHeader()
                .setSkipHeaderRecord(true)
                .setIgnoreEmptyLines(true)
                .setTrim(true)
                .build();
            return new CSVParser(reader, format);
        } catch (Exception e) {
            throw new RuntimeException("Failed to load CSV: " + classpathPath, e);
        }
    }

    /**
     * Splits a comma-separated stops/stations string (which may be quoted in
     * the CSV) into a trimmed list of node ID strings.
     * Example: "12,1,3,F2,11" → ["12", "1", "3", "F2", "11"]
     */
    private List<String> splitNodeIds(String raw) {
        List<String> result = new ArrayList<>();
        for (String token : raw.split(",")) {
            String trimmed = token.trim();
            if (!trimmed.isEmpty()) {
                result.add(trimmed);
            }
        }
        return result;
    }

    private Map<String, AbstractNode> buildNodeIndex(List<Neighborhood> neighborhoods,
                                                     List<Facility> facilities) {
        Map<String, AbstractNode> index = new LinkedHashMap<>();
        neighborhoods.forEach(node -> index.put(node.getNodeId(), node));
        facilities.forEach(node -> index.put(node.getNodeId(), node));
        return Collections.unmodifiableMap(index);
    }

    private Map<String, Road> buildRoadIndex(List<Road> roads) {
        Map<String, Road> index = new LinkedHashMap<>();
        roads.forEach(road -> index.put(road.asRoadId(), road));
        return Collections.unmodifiableMap(index);
    }

    private AbstractNode resolveNode(String nodeId) {
        AbstractNode node = nodesById.get(nodeId);
        if (node == null) {
            throw new IllegalArgumentException("Unknown node ID referenced in CSV data: " + nodeId);
        }
        return node;
    }

    private List<AbstractNode> resolveNodes(List<String> nodeIds) {
        return nodeIds.stream()
            .map(this::resolveNode)
            .toList();
    }

    private Road resolveRoad(String roadId) {
        Road road = roadsById.get(roadId);
        if (road == null) {
            throw new IllegalArgumentException("Unknown road ID referenced in CSV data: " + roadId);
        }
        return road;
    }
}
