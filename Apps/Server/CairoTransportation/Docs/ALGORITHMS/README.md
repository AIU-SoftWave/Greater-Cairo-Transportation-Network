# Algorithms

This folder explains the algorithm side of the project.

## What this section is for

The project brief requires algorithmic solutions for:
- road network design
- route planning
- emergency routing
- time-varying traffic
- transit scheduling
- maintenance planning
- traffic control

These pages explain how the current data can support those algorithms.

## Pages
- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)
- [Implementation Roadmap](ROADMAP.md)
- [Diagrams](../DIAGRAMS/README.md)

---

## How the data behaves for algorithms

### Graph-based behavior
- `locations` are vertices
- `roads` are directed edges
- `distance`, `capacity`, and `condition` influence edge cost
- `isExisting` and `constructionCost` help compare built vs. possible roads

### Time-based behavior
- `traffic_flow` changes the cost of a road depending on period
- morning and evening traffic should increase weight
- night or off-peak periods should reduce weight

### Transit behavior
- `transport_routes` describe metro and bus networks
- `route_stops` describe ordered station/stop sequences
- `transport_demand` shows where riders are concentrated

### Maintenance behavior
- `road_maintenance` helps rank roads by priority and cost
- poor condition roads can be selected first by a planning algorithm

---

## Algorithm services and future endpoints

This section explains what each algorithm service should do and what endpoint it should expose when implemented.

### 1. MST / Road Network Design

#### Planned service
`MstService` or `GraphMstService`

#### What it should do
- build a low-cost network using locations and roads
- prefer existing roads when possible
- consider population and critical facilities as priority signals
- return selected roads, total cost, and connectivity explanation

#### Planned endpoint
- `GET /api/algorithms/mst`

#### Expected response
- selected road list
- total network cost
- connected locations
- notes about critical coverage

#### Data used
- `locations`
- `roads`

---

### 2. Shortest Path / Dijkstra

#### Planned service
`ShortestPathService` or `DijkstraService`

#### What it should do
- find the best normal route between two locations
- use road distance and quality as cost factors
- return the path and total cost

#### Planned endpoint
- `GET /api/algorithms/shortest-path?from=1&to=3`

#### Expected response
- route as ordered locations or roads
- total distance/cost
- visited nodes summary

#### Data used
- `roads`
- `locations`

---

### 3. Emergency Routing / A*

#### Planned service
`EmergencyRoutingService` or `AStarService`

#### What it should do
- find a fast path to a critical facility
- prefer routes that reduce estimated response time
- use a heuristic such as geographic distance
- account for road quality and traffic if needed

#### Planned endpoint
- `GET /api/algorithms/emergency-route?from=1&to=F9`

#### Expected response
- emergency path
- estimated response time
- heuristic explanation

#### Data used
- `locations`
- `roads`
- `traffic_flow`
- critical flags on locations

---

### 4. Time-Dependent Shortest Path

#### Planned service
`TimeAwareRouteService`

#### What it should do
- adjust edge costs by traffic period
- find routes that are best for morning, evening, or night
- show how congestion changes the answer

#### Planned endpoint
- `GET /api/algorithms/time-route?from=1&to=3&period=MORNING`

#### Expected response
- route for the requested period
- time-adjusted cost
- traffic explanation

#### Data used
- `roads`
- `traffic_flow`

---

### 5. Dynamic Programming for Transit Scheduling

#### Planned service
`TransitSchedulingService`

#### What it should do
- allocate metro or bus service based on demand
- balance vehicle availability, passenger demand, and route coverage
- return an optimized schedule or allocation plan

#### Planned endpoint
- `GET /api/algorithms/transit-schedule`

#### Expected response
- route allocations
- vehicle counts
- demand coverage summary
- cost or utility score

#### Data used
- `transport_routes`
- `route_stops`
- `transport_demand`

---

### 6. Dynamic Programming for Maintenance Planning

#### Planned service
`MaintenancePlanningService`

#### What it should do
- choose which roads to repair under a budget
- prefer high-priority or badly conditioned roads
- maximize improvement while staying within budget

#### Planned endpoint
- `GET /api/algorithms/maintenance-plan?budget=1000`

#### Expected response
- selected roads
- total estimated cost
- total priority value
- budget usage

#### Data used
- `road_maintenance`
- `roads.condition`

---

### 7. Greedy Traffic Signal Optimization

#### Planned service
`TrafficSignalService`

#### What it should do
- choose the intersection or direction that gives the best immediate congestion relief
- act quickly for high-traffic situations
- be easy to compute in real time

#### Planned endpoint
- `GET /api/algorithms/traffic-signals?period=MORNING`

#### Expected response
- prioritized intersections
- congestion scores
- short explanation of the choice

#### Data used
- `traffic_flow`
- `roads`

---

### 8. Greedy Emergency Priority Handling

#### Planned service
`EmergencyPriorityService`

#### What it should do
- give priority to emergency vehicles near critical facilities
- reduce waiting time during congestion
- use simple real-time rules

#### Planned endpoint
- `GET /api/algorithms/emergency-priority?from=1&to=F10`

#### Expected response
- priority decision
- route or intersection priority list
- justification

#### Data used
- `locations`
- `roads`
- `traffic_flow`
- critical flags

---

## Service design notes

Each future algorithm service should:
- do one job only
- use the current EF Core data model
- return a small result object or DTO
- be called from one controller endpoint
- be documented here

## What the algorithms should produce

Future algorithm endpoints should return structured results such as:
- selected roads or stations
- total cost
- path or route steps
- travel time or distance
- explanation of why the result was chosen

---

## Beginner summary

The database is not just storage.
It is the input to the optimization algorithms.
The algorithms turn raw city data into decisions.

## More reading
- [Graph Data Behavior](GRAPH-DATA.md)
- [MST](MST.md)
- [Shortest Path](SHORTEST-PATH.md)
- [Dynamic Programming](DYNAMIC-PROGRAMMING.md)
- [Greedy Methods](GREEDY.md)
- [Diagrams](../DIAGRAMS/README.md)