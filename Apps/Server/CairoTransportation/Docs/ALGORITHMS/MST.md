# Minimum Spanning Tree

## Purpose

The MST algorithm is used to design a low-cost road network.
It should choose the most useful set of roads that connects the important locations.

## How the data fits MST

The algorithm will use:
- `locations` as graph nodes
- `roads` as graph edges
- `distance` and `constructionCost` as edge weights
- `isCritical` and `population` as priority signals

## Expected behavior

The algorithm should:
- connect all required locations
- prefer cheaper connections when possible
- avoid unnecessary edges
- give special attention to critical facilities

## Possible adjustments for this project

A plain MST is not enough by itself.
The project may need weighted priority rules such as:
- critical facilities get higher priority
- high-population locations are preferred earlier
- existing roads may be treated as already available or cheaper than new roads

## Service design

### Planned service name
- `MstService`
- or `GraphMstService` if grouped under graph algorithms

### What the service should do
- load the road graph from the database
- score existing and potential roads
- select the best set of connections
- compute the total cost
- return a structured result object

### What the service should not do
- return raw database entities directly
- mix UI or controller logic with graph logic
- write to the database unless saving a scenario result is needed

## Planned endpoint
- `GET /api/algorithms/mst`

## Output ideas
The result should include:
- selected roads
- total network cost
- connected locations
- explanation of why each edge was chosen

## Beginner summary
Think of MST as a way to connect the city with the smallest possible cost while still keeping the network connected.

## Related pages
- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [Diagrams](../DIAGRAMS/README.md)