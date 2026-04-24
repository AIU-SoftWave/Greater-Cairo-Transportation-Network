# Dynamic Programming

## Purpose

Dynamic programming is used when the project must optimize a choice by breaking it into smaller subproblems.

## Where it can be used
- bus and metro scheduling
- road maintenance planning
- resource allocation
- memoized route subproblems

## Transit scheduling

The algorithm can decide how many vehicles or trips should be assigned to routes.
It should balance:
- passenger demand
- available vehicles
- route coverage
- operating cost

## Maintenance planning

The algorithm can choose which roads to repair under a limited budget.
It should balance:
- condition
- priority
- estimated cost
- strategic importance

## How the data fits
- `transport_routes` and `route_stops` describe transit structure
- `transport_demand` describes demand pressure
- `road_maintenance` describes repair candidates
- `roads.condition` can also help rank repairs

## Why DP helps
DP is useful when the best answer depends on smaller best answers.
It avoids recomputing the same subproblems over and over.

## Service design

### Planned services
- `TransitSchedulingService`
- `MaintenancePlanningService`

### What each service should do
- **TransitSchedulingService**: decide how to assign buses or metro frequency to satisfy demand better
- **MaintenancePlanningService**: choose the best set of road repairs under a budget constraint

### What the services should return
- chosen schedule or repair plan
- total cost
- total expected benefit
- explanation of the chosen combination

## Planned endpoints
- `GET /api/algorithms/transit-schedule`
- `GET /api/algorithms/maintenance-plan?budget=1000`

## Beginner summary
Think of dynamic programming as an organized way to search many choices without repeating work.

## Related pages
- [Overview](OVERVIEW.md)
- [Graph Data Behavior](GRAPH-DATA.md)
- [Diagrams](../DIAGRAMS/README.md)