# Greater Cairo Transportation Network – Project Overview

## Table of Contents
1. [Project Purpose](#1-project-purpose)
2. [Repository Layout](#2-repository-layout)
3. [Architecture](#3-architecture)
4. [Data Schemas](#4-data-schemas)
5. [API Reference](#5-api-reference)
6. [Design Decisions](#6-design-decisions)
7. [Algorithm Roadmap](#7-algorithm-roadmap)
8. [How to Run](#8-how-to-run)

> **v2.0 – Modular Monolith**: the codebase was refactored from a flat-package MVC layout into a
> **modular monolith**. Each domain module (`neighborhood`, `facility`, `road`, `traffic`, `transit`)
> is self-contained with its own `model`, `repository`, `service`, and `controller` sub-packages.
> Shared infrastructure lives in the `shared` module. All controllers expose only the minimal
> **GET /all** and **GET /{id}** endpoints.

---

## 1. Project Purpose

Cairo is one of the world's largest metropolitan areas and faces serious challenges:

- Chronic traffic congestion, especially during morning (07-09) and evening (16-19) rush hours
- A public transit network (metro + buses) that struggles to meet demand
- Ageing road infrastructure with variable quality
- High cost of building new roads, requiring careful cost-benefit analysis

This project builds a **REST API** that makes all of Cairo's transportation data available in a clean, structured form.  
The API is the backbone for the algorithmic modules (shortest path, MST, dynamic programming, greedy scheduling) that are described in the project brief.

---

## 2. Repository Layout

```
Greater-Cairo-Transportation-Network/
├── Docs/
│   ├── CSE112-Practical Project.txt   ← project brief
│   └── Project_Provided_Data.txt      ← raw data reference
└── Apps/
    └── transport-system-server/        ← Spring Boot application
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
            │   │   ├── service/             (RoadService)
            │   │   └── controller/          (RoadController, PotentialRoadController)
            │   ├── traffic/                 ← Traffic module
            │   │   ├── model/               (TrafficPattern)
            │   │   ├── repository/          (TrafficPatternRepository)
            │   │   ├── service/             (TrafficService)
            │   │   └── controller/          (TrafficController)
            │   └── transit/                 ← Transit module
            │       ├── model/               (MetroLine, BusRoute, TransitDemand)
            │       ├── repository/          (MetroLineRepository, BusRouteRepository, TransitDemandRepository)
            │       ├── service/             (MetroService, BusService, DemandService)
            │       └── controller/          (MetroController, BusController, DemandController)
            └── resources/
                ├── application.properties
                └── static/data/    ← all CSV files (the data store)
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

The application follows a **Modular Monolith** pattern. Each domain module is self-contained
and deployed as part of a single Spring Boot application while maintaining clear internal
module boundaries.

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
| **road** | `com.softwave.transportsystem.road` | `Road`, `PotentialRoad`, `RoadRepository`, `PotentialRoadRepository`, `RoadService`, `RoadController`, `PotentialRoadController` |
| **traffic** | `com.softwave.transportsystem.traffic` | `TrafficPattern`, `TrafficPatternRepository`, `TrafficService`, `TrafficController` |
| **transit** | `com.softwave.transportsystem.transit` | `MetroLine`, `BusRoute`, `TransitDemand`, their repositories, `MetroService`, `BusService`, `DemandService`, `MetroController`, `BusController`, `DemandController` |

### Layer responsibilities

| Layer | Responsibility |
|---|---|
| **Model** | Plain JPA entities that mirror the database tables. No behaviour, just data. |
| **Repository** | Spring Data JPA interfaces. Each module owns its own repositories. |
| **Service** | Business logic. Controllers never access repositories directly. |
| **Controller** | Maps HTTP requests to service calls. Exposes only `GET /all` and `GET /{id}`. |

---

## 4. Data Schemas

All files live in `src/main/resources/static/data/`.

---

### 4.1 nodes.csv – Neighborhoods / Districts

| Column | Type | Description |
|---|---|---|
| `ID` | integer | Unique node identifier (1–15) |
| `Name` | string | Human-readable district name |
| `Population` | integer | Estimated resident population |
| `Type` | enum | `Residential`, `Mixed`, `Business`, `Industrial`, `Government` |
| `Longitude` | decimal | WGS-84 longitude (east-west) |
| `Latitude` | decimal | WGS-84 latitude (north-south) |

**Sample rows**

| ID | Name | Population | Type | Longitude | Latitude |
|---|---|---|---|---|---|
| 1 | Maadi | 250,000 | Residential | 31.25 | 29.96 |
| 3 | Downtown Cairo | 100,000 | Business | 31.24 | 30.04 |
| 8 | Giza | 550,000 | Mixed | 31.21 | 29.99 |

> **Why do we store population and type?**  
> MST algorithms that prioritise high-density connections need population data.  
> The `type` field lets us query which nodes are hospitals/government centres and must have guaranteed connectivity.

---

### 4.2 facilities.csv – Important Facilities

| Column | Type | Description |
|---|---|---|
| `ID` | string | Identifier prefixed with "F" (F1–F10) |
| `Name` | string | Full facility name |
| `Type` | enum | `Airport`, `Transit Hub`, `Education`, `Tourism`, `Sports`, `Business`, `Commercial`, `Medical` |
| `Longitude` | decimal | WGS-84 longitude |
| `Latitude` | decimal | WGS-84 latitude |

Facilities share the same coordinate space as neighborhoods and appear as nodes in the road and transit networks.  
Medical facilities (F9, F10) and transit hubs (F2) are especially important for emergency routing and metro planning.

---

### 4.3 existing_roads.csv – Current Road Network

| Column | Type | Description |
|---|---|---|
| `FromID` | string | Source node (integer or "F…") |
| `ToID` | string | Destination node |
| `Distance_km` | decimal | Road length in kilometres |
| `Capacity_vph` | integer | Maximum flow in vehicles per hour |
| `Condition` | integer | Road quality 1–10 (10 = perfect, ≤4 = needs maintenance) |

The road network is a **directed graph** where each row is an edge.  
Most roads are represented once (one direction), so treat the graph as **undirected** when running MST or unweighted shortest-path algorithms.

> **Condition score** drives the DP resource-allocation problem: roads with condition ≤ 5 are candidates for maintenance budget allocation.

---

### 4.4 potential_roads.csv – Proposed New Roads

| Column | Type | Description |
|---|---|---|
| `FromID` | string | Source node |
| `ToID` | string | Destination node |
| `Distance_km` | decimal | Projected length |
| `Capacity_vph` | integer | Projected capacity after construction |
| `Construction_Cost_Million_EGP` | integer | Estimated cost in millions of Egyptian Pounds |

These are the candidate edges fed into Kruskal's/Prim's MST algorithm when planning new road construction.  
Sorting by `Construction_Cost_Million_EGP` ascending gives a greedy cheapest-first build order.

---

### 4.5 traffic_patterns.csv – Time-of-Day Traffic Volumes

| Column | Type | Description |
|---|---|---|
| `RoadID` | string | Road identified as "FromID-ToID" (e.g. `"1-3"`, `"F1-5"`) |
| `Morning_Peak_vph` | integer | Volume 07:00–09:00 |
| `Afternoon_vph` | integer | Volume 12:00–14:00 |
| `Evening_Peak_vph` | integer | Volume 16:00–19:00 |
| `Night_vph` | integer | Volume 22:00–05:00 |

> **Why four time slots?**  
> Cairo's Dijkstra/A* implementations need **time-varying edge weights**.  
> At peak hours, a road operating near its capacity receives a higher effective travel-time cost than the same road at night.  
> The formula used is: `effective_cost = distance_km * (volume / capacity)` — the closer to saturation, the longer the expected delay.

---

### 4.6 metro_lines.csv – Metro Lines

| Column | Type | Description |
|---|---|---|
| `LineID` | string | `M1`, `M2`, `M3` |
| `Name` | string | Descriptive name with termini |
| `Stations` | comma list | Ordered node IDs from one terminus to the other |
| `Daily_Passengers` | integer | Average daily ridership |

The three lines carry **3.5 million passengers per day** combined, making them the backbone of public transit.  
The DP scheduling module uses `Daily_Passengers` and the length of the `Stations` list to calculate train frequency requirements.

---

### 4.7 bus_routes.csv – Bus Routes

| Column | Type | Description |
|---|---|---|
| `RouteID` | string | `B1`–`B10` |
| `Stops` | comma list | Ordered node IDs along the route |
| `Buses_Assigned` | integer | Number of buses currently operating |
| `Daily_Passengers` | integer | Average daily ridership |

Ten routes with 12–30 buses each.  
The DP optimal-scheduling problem asks: given a fixed fleet, how should buses be redistributed across routes to maximise total coverage?

---

### 4.8 transit_demand.csv – Origin-Destination Demand

| Column | Type | Description |
|---|---|---|
| `FromID` | string | Origin node |
| `ToID` | string | Destination node |
| `Daily_Passengers` | integer | Daily trips on this OD pair |

17 OD pairs capturing the dominant passenger flows (e.g. 22 000 daily trips from Giza to Downtown Cairo).  
This matrix is the input to the public-transit optimisation: routes and frequencies should be designed so that high-demand corridors have sufficient capacity.

---

## 5. API Reference

Start the server (`mvn spring-boot:run` from the `transport-system-server` directory) then open `http://localhost:8080/`.

### 5.1 Root

| Method | Path | Description |
|---|---|---|
| `GET` | `/` | Returns a JSON directory of all endpoints |

---

### 5.2 Neighborhoods `/api/neighborhoods`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/neighborhoods` | All districts |
| `GET` | `/api/neighborhoods/{id}` | Single district by numeric ID |

---

### 5.3 Facilities `/api/facilities`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/facilities` | All facilities |
| `GET` | `/api/facilities/{id}` | Single facility (e.g. `/api/facilities/F9`) |

---

### 5.4 Roads `/api/roads`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/roads` | All existing roads |
| `GET` | `/api/roads/{id}` | Single road by numeric ID |

---

### 5.5 Potential Roads `/api/potential-roads`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/potential-roads` | All proposed new roads |
| `GET` | `/api/potential-roads/{id}` | Single proposed road by numeric ID |

---

### 5.6 Traffic `/api/traffic`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/traffic` | All time-of-day traffic patterns |
| `GET` | `/api/traffic/{id}` | Single traffic pattern by numeric ID |

---

### 5.7 Metro `/api/metro`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/metro` | All metro lines |
| `GET` | `/api/metro/{id}` | One metro line (e.g. `M2`) |

---

### 5.8 Bus `/api/bus`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/bus` | All bus routes |
| `GET` | `/api/bus/{id}` | One bus route (e.g. `B4`) |

---

### 5.9 Transit Demand `/api/demand`

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/demand` | All OD demand records |
| `GET` | `/api/demand/{id}` | Single OD demand record by numeric ID |

---

## 6. Design Decisions

### Why Modular Monolith?
A modular monolith keeps the **simplicity of a single deployable** while enforcing clear domain
boundaries.  Each module owns its own stack (model → repository → service → controller) and
communicates with other modules only through well-defined service APIs, not by reaching into
another module's internals.  This makes the codebase easy to understand, test, and — if needed —
migrate to separate microservices later.

### Why Spring Boot?
Spring Boot auto-configures the web layer, JSON serialisation (Jackson), and the application lifecycle.  
A team can get a working REST API running in minutes without writing any boilerplate HTTP server code.

### Why H2 + Flyway?
The project seeds a small, fixed dataset into an embedded H2 database at startup via
`CsvDatabaseSeeder`.  Flyway manages the schema migration so the DDL is version-controlled and
repeatable.  No external database or Docker setup is required.

### Why SOLID?

| Principle | How it is applied |
|---|---|
| **S** – Single Responsibility | Each class has exactly one job: models hold data, repositories query data, services contain logic, controllers handle HTTP. |
| **O** – Open/Closed | New algorithms can be added as new service methods without modifying existing ones. |
| **L** – Liskov | All service classes are concrete; the Spring interfaces they implement (`@Service`) behave identically. |
| **I** – Interface Segregation | Controllers only depend on the service they need, not a monolithic "god service". |
| **D** – Dependency Inversion | All dependencies are injected via constructors; no `new` operator inside classes. |

### Why constructor injection (not `@Autowired` on fields)?
Constructor injection makes dependencies explicit, improves testability (no Spring context needed in unit tests), and prevents null-injection errors at startup.

---

## 7. Algorithm Roadmap

The API provides all the data these algorithms need:

| Algorithm | Input data | Purpose |
|---|---|---|
| **Dijkstra's** shortest path | `existing_roads` (weight = `distance_km`) | Standard route planning between any two nodes |
| **A*** emergency routing | `existing_roads` + lat/lng for heuristic | Fastest path for ambulances to reach hospitals |
| **Time-varying Dijkstra** | `traffic_patterns` (weight = `distance × (volume/capacity)`) | Route planning that avoids peak-hour congestion |
| **Kruskal's / Prim's MST** | `potential_roads` (weight = `construction_cost`) | Minimum-cost network connecting all areas |
| **DP scheduling** | `bus_routes`, `metro_lines`, `transit_demand` | Optimal fleet / frequency allocation |
| **DP maintenance** | `existing_roads` (weight = `condition`) | Budget allocation for road repairs |
| **Greedy signal timing** | `traffic_patterns` | Real-time traffic-light control at intersections |

---

## 8. How to Run

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
Open that URL in a browser or use `curl` / Postman.

### Quick smoke-test

```bash
# Directory of all endpoints
curl http://localhost:8080/

# All neighborhoods
curl http://localhost:8080/api/neighborhoods

# Neighborhood by ID
curl http://localhost:8080/api/neighborhoods/1

# All facilities
curl http://localhost:8080/api/facilities

# Facility by ID
curl http://localhost:8080/api/facilities/F9

# All existing roads
curl http://localhost:8080/api/roads

# All proposed roads
curl http://localhost:8080/api/potential-roads

# All traffic patterns
curl http://localhost:8080/api/traffic

# All metro lines
curl http://localhost:8080/api/metro

# All bus routes
curl http://localhost:8080/api/bus

# All transit demand records
curl http://localhost:8080/api/demand
```
