# Algorithm and Service Naming Plan

## Goal

Make the codebase easy to read by separating:
- business services
- pure algorithm classes
- shared helper utilities

The naming should make it obvious:
- what the business feature is
- what algorithm is used
- where each piece belongs

## Naming Rules

### 1. Business services
Use the business purpose first.

Examples:
- `RoutePlanningService`
- `EmergencyRoutingService`
- `NetworkExpansionService`
- `TrafficMonitoringService`
- `SignalOptimizationService`
- `MaintenancePlanningService`
- `TransitOperationsService`

These services are the business-facing layer.
They coordinate input validation, data loading, and response shaping.

### 2. Algorithm classes
Use the algorithm name plus the business intent.

Examples:
- `DijkstraRoutePlanner`
- `AStarPathFinder`
- `PrimNetworkExpander`
- `KnapsackMaintenanceOptimizer`
- `ResourceAllocationScheduler`
- `GreedySignalOptimizer`
- `TimeVaryingRoutePlanner`

These classes should contain pure logic only.
They should not know about controllers, HTTP, or database access.

### 3. Helper utilities
Use utility names only for small reusable helpers.

Examples:
- `GraphCostCalculator`
- `RouteNormalizationHelper`
- `PriorityQueueExtensions`
- `TimePeriodHelper`

Helpers should support algorithms, not replace them.

## Layer Responsibilities

### Business layer
Responsible for:
- reading data from the database
- validating input
- choosing the right algorithm
- returning a response DTO

### Algorithm layer
Responsible for:
- running the actual algorithm
- working on plain input objects
- returning a result object

### Utility layer
Responsible for:
- small reusable helpers
- math and normalization functions
- shared low-level logic

## Recommended Folder Layout

```text
Algorithms/
  ShortestPath/
    DijkstraRoutePlanner.cs
    AStarPathFinder.cs
    TimeVaryingRoutePlanner.cs
  Greedy/
    GreedySignalOptimizer.cs
  DynamicProgramming/
    KnapsackMaintenanceOptimizer.cs
    ResourceAllocationScheduler.cs
  NetworkExpansion/
    PrimNetworkExpander.cs
```

## Folder Grouping Rule

Group algorithm folders by the algorithm family first, then place the concrete implementation inside that family folder.

Examples:
- `ShortestPath/` for Dijkstra, A*, and time-varying shortest path
- `Greedy/` for greedy optimization logic
- `DynamicProgramming/` for knapsack and resource allocation DP
- `NetworkExpansion/` for MST / Prim-based expansion

This keeps the tree readable while still showing what algorithm family each business feature uses.

## Recommended Service Mapping

- `RoutePlanningService` -> `DijkstraRoutePlanner`
- `EmergencyRoutingService` -> `AStarPathFinder`
- `TimeAwareRoutePlanningService` -> `TimeVaryingRoutePlanner`
- `NetworkExpansionService` -> `PrimNetworkExpander`
- `MaintenancePlanningService` -> `KnapsackMaintenanceOptimizer`
- `TransitOperationsService` -> `ResourceAllocationScheduler`
- `SignalOptimizationService` -> `GreedySignalOptimizer`

## Example Pattern

```csharp
public class EmergencyRoutingService : IEmergencyRoutingService
{
    private readonly IAStarPathFinder pathFinder;

    public EmergencyRoutingService(IAStarPathFinder pathFinder)
    {
        this.pathFinder = pathFinder;
    }
}
```

This keeps the business service readable while the algorithm remains reusable and testable.

## What to Avoid

- Do not put database access inside algorithm classes.
- Do not return HTTP results from algorithm classes.
- Do not hide algorithms under generic utility names.
- Do not name business services after algorithms only.

## Practical Rule

If the class answers "what business problem is this solving?", it belongs in the service layer.
If the class answers "what algorithm is being used?", it belongs in the algorithm layer.
