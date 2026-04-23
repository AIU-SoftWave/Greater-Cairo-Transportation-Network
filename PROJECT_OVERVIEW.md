# Greater Cairo Transportation Network – Project Overview

## Table of Contents
1. [Project Purpose](#1-project-purpose)
2. [Repository Layout](#2-repository-layout)
3. [Architecture](#3-architecture)
4. [Data Schemas](#4-data-schemas)
5. [API Reference](#5-api-reference)
6. [Algorithm Implementation Checklist](#6-algorithm-implementation-checklist)
7. [How to Add Your Own Algorithm Module](#7-how-to-add-your-own-algorithm-module)
8. [Design Decisions](#8-design-decisions)
9. [Diagrams](#9-diagrams)
10. [How to Run](#10-how-to-run)

> **v2.0 – Modular Monolith**: the codebase was refactored from a flat-package MVC layout into a
> **modular monolith**. Each domain module (`neighborhood`, `facility`, `road`, `traffic`, `transit`,
> `graph`) is self-contained with its own `model`, `repository`, `service`, and `controller`
> sub-packages. Shared infrastructure lives in the `shared` module. Controllers expose
> **GET /all** and **GET /{id}** endpoints for data access, plus one endpoint per algorithm.

---

## 1. Project Purpose

Cairo is one of the world's largest metropolitan areas and faces serious challenges:

- Chronic traffic congestion, especially during morning (07–09) and evening (16–19) rush hours
- A public transit network (metro + buses) that struggles to meet demand
- Ageing road infrastructure with variable quality
- High cost of building new roads, requiring careful cost-benefit analysis

This project builds a **REST API** that makes all of Cairo's transportation data available in a
clean, structured form. The API serves as the backbone for the algorithmic modules
(shortest path, MST, dynamic programming, greedy scheduling) described in the project brief.

Each student or sub-team implements exactly one algorithm module by following the steps in
[Section 7](#7-how-to-add-your-own-algorithm-module).

---

## 2. Repository Layout

```
Greater-Cairo-Transportation-Network/
├── Docs/
│   ├── CSE112-Practical Project.txt        ← project brief
│   ├── Project_Provided_Data.txt           ← raw data reference
│   └── diagrams/                           ← PlantUML source files
│       ├── entity-class-diagram.puml       ← JPA entity hierarchy
│       ├── database-schema.puml            ← H2 table ER diagram
│       ├── module-architecture.puml        ← component / module diagram
│       └── api-flow-sequence.puml          ← Dijkstra request sequence
└── Apps/
    └── transport-system-server/            ← Spring Boot application
        ├── pom.xml
        └── src/main/
            ├── java/com/softwave/transportsystem/
            │   ├── TransportSystemServerApplication.java   ← entry point
            │   ├── HomeController.java                     ← API directory
            │   ├── shared/                  ← cross-cutting infrastructure
            │   │   ├── model/               (AbstractNode, AbstractEdge)
            │   │   ├── repository/          (NodeRepository)
            │   │   └── seeder/              (CsvDatabaseSeeder)
            │   ├── neighborhood/            ← Neighborhood module
            │   │   ├── model/               (Neighborhood)
            │   │   ├── repository/          (NeighborhoodRepository)
            │   │   ├── service/             (NeighborhoodService)
            │   │   └── controller/          (NeighborhoodController)
            │   ├── facility/                ← Facility module
            │   │   ├── model/               (Facility)
            │   │   ├── repository/          (FacilityRepository)
            │   │   ├── service/             (FacilityService)
            │   │   └── controller/          (FacilityController)
            │   ├── road/                    ← Road & PotentialRoad module
            │   │   ├── model/               (Road, PotentialRoad)
            │   │   ├── repository/          (RoadRepository, PotentialRoadRepository)
            │   │   ├── service/             (RoadService, DpMaintenanceService [PLACEHOLDER])
            │   │   └── controller/          (RoadController, PotentialRoadController)
            │   ├── traffic/                 ← Traffic module
            │   │   ├── model/               (TrafficPattern)
            │   │   ├── repository/          (TrafficPatternRepository)
            │   │   ├── service/             (TrafficService, GreedySignalTimingService [PLACEHOLDER])
            │   │   └── controller/          (TrafficController)
            │   ├── transit/                 ← Transit module
            │   │   ├── model/               (MetroLine, BusRoute, TransitDemand)
            │   │   ├── repository/          (MetroLineRepository, BusRouteRepository, TransitDemandRepository)
            │   │   ├── service/             (MetroService, BusService, DemandService, DpSchedulingService [PLACEHOLDER])
            │   │   └── controller/          (MetroController, BusController, DemandController)
            │   └── graph/                   ← Graph algorithm module
            │       ├── model/               (GraphEdge, ShortestPathResult, MstResult)
            │       ├── service/             (GraphService, DijkstraService [DONE],
            │       │                         KruskalMstService [DONE], AStarService [PLACEHOLDER],
            │       │                         TimeVaryingDijkstraService [PLACEHOLDER],
            │       │                         PrimMstService [PLACEHOLDER])
            │       └── controller/          (GraphController)
            └── resources/
                ├── application.properties
                ├── db/migration/            ← Flyway SQL migrations
                │   └── V1__initial_schema.sql
                └── static/data/            ← all CSV files (the data store)
                    ├── nodes.csv
                    ├── facilities.csv
                    ├── existing_roads.csv
                    ├── potential_roads.csv
                    ├── traffic_patterns.csv
                    ├── metro_lines.csv
                    ├── bus_routes.csv
                    └── transit_demand.csv
```

---

## 3. Architecture

The application follows a **Modular Monolith** pattern:

```
HTTP Request
     │
     ▼
┌─────────────┐   calls   ┌─────────────┐   reads   ┌──────────────────┐
│  Controller │ ────────► │   Service   │ ────────► │   Repository     │
│ (REST layer)│           │(business    │           │  (JPA / H2 DB)   │
└─────────────┘           │  logic)     │           └──────────────────┘
                          └─────────────┘                    ▲
                                                             │ seeded once at startup
                                                             │
                                                  ┌──────────────────┐
                                                  │  CsvDatabaseSeeder│
                                                  │  (CSV → H2 DB)   │
                                                  └──────────────────┘
```

### Module Breakdown

| Module | Package | Contents |
|---|---|---|
| **shared** | `com.softwave.transportsystem.shared` | `AbstractNode`, `AbstractEdge`, `NodeRepository`, `CsvDatabaseSeeder` |
| **neighborhood** | `com.softwave.transportsystem.neighborhood` | `Neighborhood`, `NeighborhoodRepository`, `NeighborhoodService`, `NeighborhoodController` |
| **facility** | `com.softwave.transportsystem.facility` | `Facility`, `FacilityRepository`, `FacilityService`, `FacilityController` |
| **road** | `com.softwave.transportsystem.road` | `Road`, `PotentialRoad`, `RoadService`, `DpMaintenanceService`, `RoadController`, `PotentialRoadController` |
| **traffic** | `com.softwave.transportsystem.traffic` | `TrafficPattern`, `TrafficService`, `GreedySignalTimingService`, `TrafficController` |
| **transit** | `com.softwave.transportsystem.transit` | `MetroLine`, `BusRoute`, `TransitDemand`, their repositories, `MetroService`, `BusService`, `DemandService`, `DpSchedulingService`, `MetroController`, `BusController`, `DemandController` |
| **graph** | `com.softwave.transportsystem.graph` | `GraphEdge`, `ShortestPathResult`, `MstResult`, `GraphService`, `DijkstraService`, `KruskalMstService`, `AStarService`, `TimeVaryingDijkstraService`, `PrimMstService`, `GraphController` |

### Layer responsibilities

| Layer | Responsibility |
|---|---|
| **Model** | Plain JPA entities that mirror the database tables. No behaviour, just data. |
| **Repository** | Spring Data JPA interfaces. Each module owns its own repositories. |
| **Service** | Business logic. Controllers never access repositories directly. |
| **Controller** | Maps HTTP requests to service calls. |

---

## 4. Data Schemas

All CSV files live in `src/main/resources/static/data/`.

### 4.1 nodes.csv – Neighborhoods / Districts

| Column | Type | Description |
|---|---|---|
| `ID` | integer | Unique node identifier (1–15) |
| `Name` | string | Human-readable district name |
| `Population` | integer | Estimated resident population |
| `Type` | enum | `Residential`, `Mixed`, `Business`, `Industrial`, `Government` |
| `Longitude` | decimal | WGS-84 longitude |
| `Latitude` | decimal | WGS-84 latitude |

### 4.2 facilities.csv – Important Facilities

| Column | Type | Description |
|---|---|---|
| `ID` | string | Identifier prefixed with "F" (F1–F10) |
| `Name` | string | Full facility name |
| `Type` | enum | `Airport`, `Transit Hub`, `Education`, `Tourism`, `Sports`, `Business`, `Commercial`, `Medical` |
| `Longitude` | decimal | WGS-84 longitude |
| `Latitude` | decimal | WGS-84 latitude |

### 4.3 existing_roads.csv – Current Road Network

| Column | Type | Description |
|---|---|---|
| `FromID` | string | Source node (integer or "F...") |
| `ToID` | string | Destination node |
| `Distance_km` | decimal | Road length in kilometres |
| `Capacity_vph` | integer | Maximum flow in vehicles per hour |
| `Condition` | integer | Road quality 1–10 (10 = perfect, ≤ 4 = needs maintenance) |

### 4.4 potential_roads.csv – Proposed New Roads

| Column | Type | Description |
|---|---|---|
| `FromID` | string | Source node |
| `ToID` | string | Destination node |
| `Distance_km` | decimal | Projected length |
| `Capacity_vph` | integer | Projected capacity after construction |
| `Construction_Cost_Million_EGP` | integer | Estimated cost in millions of Egyptian Pounds |

### 4.5 traffic_patterns.csv – Time-of-Day Traffic Volumes

| Column | Type | Description |
|---|---|---|
| `RoadID` | string | Road identified as "FromID-ToID" (e.g. `"1-3"`) |
| `Morning_Peak_vph` | integer | Volume 07:00–09:00 |
| `Afternoon_vph` | integer | Volume 12:00–14:00 |
| `Evening_Peak_vph` | integer | Volume 16:00–19:00 |
| `Night_vph` | integer | Volume 22:00–05:00 |

> **Why four time slots?**
> Cairo's congestion-aware routing needs **time-varying edge weights**.
> Formula: `effective_cost = distance_km * (volume_vph / capacity_vph)`.

### 4.6 metro_lines.csv – Metro Lines

| Column | Type | Description |
|---|---|---|
| `LineID` | string | `M1`, `M2`, `M3` |
| `Name` | string | Descriptive name with termini |
| `Stations` | comma list | Ordered node IDs from one terminus to the other |
| `Daily_Passengers` | integer | Average daily ridership |

### 4.7 bus_routes.csv – Bus Routes

| Column | Type | Description |
|---|---|---|
| `RouteID` | string | `B1`–`B10` |
| `Stops` | comma list | Ordered node IDs along the route |
| `Buses_Assigned` | integer | Number of buses currently operating |
| `Daily_Passengers` | integer | Average daily ridership |

### 4.8 transit_demand.csv – Origin-Destination Demand

| Column | Type | Description |
|---|---|---|
| `FromID` | string | Origin node |
| `ToID` | string | Destination node |
| `Daily_Passengers` | integer | Daily trips on this OD pair |

---

## 5. API Reference

Start the server (`mvn spring-boot:run`) then open `http://localhost:8080/`.

### 5.1 Root

| Method | Path | Description |
|---|---|---|
| `GET` | `/` | Returns a JSON directory of all endpoints |

### 5.2 Neighborhoods `/api/neighborhoods`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/neighborhoods` | All districts |
| `GET` | `/api/neighborhoods/{id}` | Single district by numeric ID |

### 5.3 Facilities `/api/facilities`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/facilities` | All facilities |
| `GET` | `/api/facilities/{id}` | Single facility (e.g. `/api/facilities/F9`) |

### 5.4 Roads `/api/roads`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/roads` | All existing roads |
| `GET` | `/api/roads/{id}` | Single road by numeric ID |
| `GET` | `/api/roads/maintenance-plan?budget={millions}` | **[PLACEHOLDER]** DP maintenance budget |

### 5.5 Potential Roads `/api/potential-roads`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/potential-roads` | All proposed new roads |
| `GET` | `/api/potential-roads/{id}` | Single proposed road by numeric ID |

### 5.6 Traffic `/api/traffic`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/traffic` | All time-of-day traffic patterns |
| `GET` | `/api/traffic/{id}` | Single traffic pattern by numeric ID |
| `GET` | `/api/traffic/signal-timing?timeSlot={slot}` | **[PLACEHOLDER]** Greedy signal timing |

### 5.7 Metro `/api/metro`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/metro` | All metro lines |
| `GET` | `/api/metro/{id}` | One metro line (e.g. `M2`) |
| `GET` | `/api/metro/frequency-optimisation` | **[PLACEHOLDER]** DP metro frequency scheduling |

### 5.8 Bus `/api/bus`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/bus` | All bus routes |
| `GET` | `/api/bus/{id}` | One bus route (e.g. `B4`) |
| `GET` | `/api/bus/fleet-optimisation` | **[PLACEHOLDER]** DP bus fleet scheduling |

### 5.9 Transit Demand `/api/demand`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/demand` | All OD demand records |
| `GET` | `/api/demand/{id}` | Single OD demand record by numeric ID |

### 5.10 Graph Algorithms `/api/graph`

| Method | Path | Status | Description |
|---|---|---|---|
| `GET` | `/api/graph/shortest-path?from={id}&to={id}` | ✅ **Implemented** | Dijkstra shortest path (`weight = distance_km`) |
| `GET` | `/api/graph/mst` | ✅ **Implemented** | Kruskal MST of existing roads |
| `GET` | `/api/graph/astar?from={id}&to={id}` | ⬜ Placeholder | A* emergency routing |
| `GET` | `/api/graph/time-varying-shortest-path?from={id}&to={id}&timeSlot={slot}` | ⬜ Placeholder | Congestion-aware Dijkstra |
| `GET` | `/api/graph/prim-mst` | ⬜ Placeholder | Prim's MST on potential roads |

---

## 6. Algorithm Implementation Checklist

> ✅ = fully implemented and tested
> ⬜ = placeholder – service returns `"Not implemented: <Algorithm Name>"`

### 6.1 Shortest Path Algorithms

| # | Algorithm | Endpoint | Status | Owner |
|---|---|---|---|---|
| 1 | **Dijkstra's Shortest Path** | `GET /api/graph/shortest-path` | ✅ Implemented | SoftWave core team |
| 2 | **A\* Emergency Routing** | `GET /api/graph/astar` | ⬜ Placeholder | _assign to student_ |
| 3 | **Time-Varying Dijkstra** | `GET /api/graph/time-varying-shortest-path` | ⬜ Placeholder | _assign to student_ |

### 6.2 Minimum Spanning Tree Algorithms

| # | Algorithm | Endpoint | Status | Owner |
|---|---|---|---|---|
| 4 | **Kruskal's MST** (existing roads) | `GET /api/graph/mst` | ✅ Implemented | SoftWave core team |
| 5 | **Prim's MST** (potential roads) | `GET /api/graph/prim-mst` | ⬜ Placeholder | _assign to student_ |

### 6.3 Dynamic Programming

| # | Algorithm | Endpoint | Status | Owner |
|---|---|---|---|---|
| 6 | **DP Road Maintenance** (0/1 Knapsack) | `GET /api/roads/maintenance-plan` | ⬜ Placeholder | _assign to student_ |
| 7 | **DP Bus Fleet Scheduling** | `GET /api/bus/fleet-optimisation` | ⬜ Placeholder | _assign to student_ |
| 8 | **DP Metro Frequency Scheduling** | `GET /api/metro/frequency-optimisation` | ⬜ Placeholder | _assign to student_ |

### 6.4 Greedy Algorithms

| # | Algorithm | Endpoint | Status | Owner |
|---|---|---|---|---|
| 9 | **Greedy Traffic Signal Timing** | `GET /api/traffic/signal-timing` | ⬜ Placeholder | _assign to student_ |

---

## 7. How to Add Your Own Algorithm Module

Each student implements one algorithm by replacing a placeholder service method
with a real implementation. Follow the steps below.

### Step 1 – Understand the data

Read the project brief (`Docs/CSE112-Practical Project.txt`) and the API Reference
(Section 5) to understand which CSV files your algorithm reads.

Use the existing data endpoints to fetch the data your algorithm needs.
For example, A* needs both roads and node coordinates:

```bash
curl http://localhost:8080/api/roads
curl http://localhost:8080/api/neighborhoods
curl http://localhost:8080/api/facilities
```

### Step 2 – Find the placeholder service

All placeholder services are clearly marked with `[PLACEHOLDER]` in their Javadoc.
Locate the service file for your algorithm:

| Algorithm | Service file | Method to implement |
|---|---|---|
| A* Emergency Routing | `graph/service/AStarService.java` | `findEmergencyPath(fromId, toId)` |
| Time-Varying Dijkstra | `graph/service/TimeVaryingDijkstraService.java` | `findCongestedPath(fromId, toId, timeSlot)` |
| Prim's MST | `graph/service/PrimMstService.java` | `computeMst()` |
| DP Road Maintenance | `road/service/DpMaintenanceService.java` | `allocateBudget(budgetMillionEgp)` |
| DP Bus Fleet | `transit/service/DpSchedulingService.java` | `optimizeBusFleet()` |
| DP Metro Frequency | `transit/service/DpSchedulingService.java` | `optimizeMetroFrequency()` |
| Greedy Signal Timing | `traffic/service/GreedySignalTimingService.java` | `computeSignalTiming(timeSlot)` |

### Step 3 – Inject the repositories you need

The placeholder services currently have **no constructor arguments**. Add the
repositories or other services you need via **constructor injection**:

```java
// Example: AStarService needs roads and node coordinates
@Service
public class AStarService {

    private final RoadRepository roadRepository;
    private final NodeRepository nodeRepository;

    public AStarService(RoadRepository roadRepository,
                        NodeRepository nodeRepository) {
        this.roadRepository = roadRepository;
        this.nodeRepository = nodeRepository;
    }

    public String findEmergencyPath(String fromId, String toId) {
        // TODO: implement A* here
        return "Not implemented: A* Emergency Routing";
    }
}
```

> **Rule**: never reach into another module's internals. Always go through the
> service layer. Do not call a repository belonging to another module directly
> from a controller.

### Step 4 – Change the return type when you are ready

When your algorithm is ready to return real data, change the return type of
the service method from `String` to a proper result class:

1. Create a result class in the module's `model` package
   (e.g. `AStarResult`, `MaintenancePlan`).
2. Update the service method signature.
3. Update the controller endpoint accordingly.
4. Update the entry in Section 6 of this document to ✅.

### Step 5 – Write unit tests

Create a test class in
`src/test/java/com/softwave/transportsystem/<module>/service/`.
Use `@ExtendWith(MockitoExtension.class)` and mock all repositories with
`@Mock` so the tests run without a Spring context or database.

See `DijkstraServiceTest` and `KruskalMstServiceTest` for reference examples.

### Step 6 – Update the API directory

Add the new endpoint to `HomeController.java` and mark it as `[IMPLEMENTED]`
instead of `[PLACEHOLDER]`.

---

## 8. Design Decisions

### Why Modular Monolith?
A modular monolith keeps the **simplicity of a single deployable** while enforcing
clear domain boundaries. Each module owns its own stack (model -> repository ->
service -> controller) and communicates with other modules only through well-defined
service APIs. This makes the codebase easy to understand, test, and if needed,
migrate to separate microservices later.

### Why Spring Boot?
Spring Boot auto-configures the web layer, JSON serialisation (Jackson), and the
application lifecycle. A team can get a working REST API running in minutes without
writing any boilerplate HTTP server code.

### Why H2 + Flyway?
The project seeds a small, fixed dataset into an embedded H2 database at startup via
`CsvDatabaseSeeder`. Flyway manages the schema migration so the DDL is version-controlled
and repeatable. No external database or Docker setup is required.

### Why SOLID?

| Principle | How it is applied |
|---|---|
| **S** – Single Responsibility | Each class has exactly one job: models hold data, repositories query data, services contain logic, controllers handle HTTP. |
| **O** – Open/Closed | New algorithms can be added as new service methods without modifying existing ones. |
| **L** – Liskov | All service classes are concrete; the Spring interfaces they implement (`@Service`) behave identically. |
| **I** – Interface Segregation | Controllers only depend on the service they need, not a monolithic "god service". |
| **D** – Dependency Inversion | All dependencies are injected via constructors; no `new` operator inside classes. |

### Why constructor injection (not `@Autowired` on fields)?
Constructor injection makes dependencies explicit, improves testability (no Spring
context needed in unit tests), and prevents null-injection errors at startup.

---

## 9. Diagrams

PlantUML source files are in `Docs/diagrams/`. Convert them to PNG with:

```bash
# With PlantUML jar
java -jar plantuml.jar Docs/diagrams/*.puml

# With Docker
docker run --rm -v $(pwd):/data plantuml/plantuml -tpng /data/Docs/diagrams/*.puml
```

| File | Contents |
|---|---|
| `entity-class-diagram.puml` | Full JPA entity hierarchy (inheritance, associations) |
| `database-schema.puml` | H2 table ER diagram with foreign keys |
| `module-architecture.puml` | Component diagram showing all modules and their dependencies |
| `api-flow-sequence.puml` | Sequence diagram for a Dijkstra shortest-path request |

---

## 10. How to Run

### Prerequisites
- **Java 17** (or later)
- **Maven 3.6+**
- No database or Docker required

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/AIU-SoftWave/Greater-Cairo-Transportation-Network.git
cd Greater-Cairo-Transportation-Network/Apps/transport-system-server

# 2. Build
mvn clean package

# 3. Run
mvn spring-boot:run
# OR
java -jar target/transport-system-server-0.0.1-SNAPSHOT.jar
```

The server starts on **http://localhost:8080**.
Open that URL in a browser or use `curl` / Postman to see all endpoints.

### Quick smoke-test

```bash
# API directory
curl http://localhost:8080/

# All neighborhoods
curl http://localhost:8080/api/neighborhoods

# Dijkstra shortest path: Maadi (1) to Heliopolis (5)
curl "http://localhost:8080/api/graph/shortest-path?from=1&to=5"

# Kruskal MST of existing roads
curl http://localhost:8080/api/graph/mst

# Placeholder endpoints (return "Not implemented: ...")
curl "http://localhost:8080/api/graph/astar?from=8&to=F9"
curl "http://localhost:8080/api/graph/time-varying-shortest-path?from=1&to=5&timeSlot=MORNING"
curl http://localhost:8080/api/graph/prim-mst
curl "http://localhost:8080/api/traffic/signal-timing?timeSlot=EVENING"
curl "http://localhost:8080/api/roads/maintenance-plan?budget=500"
curl http://localhost:8080/api/bus/fleet-optimisation
curl http://localhost:8080/api/metro/frequency-optimisation
```
