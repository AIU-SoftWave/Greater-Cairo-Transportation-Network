// Core Types
export interface Node {
  id: string;
  name: string;
  x: number;
  y: number;
  isCritical: boolean;
}

export interface Road {
  fromNodeId: string;
  toNodeId: string;
  distance: number;
}

export interface Trace {
  visitedNodes: number;
  expandedNodes: number;
  executionTimeMs: number;
}

export interface AlgorithmResponse<T> {
  algorithmName: string;
  success: boolean;
  message: string;
  trace: Trace;
  data: T;
}

export interface NetworkTopologyData {
  nodes: Node[];
  edges: Road[];
  adjacencyList: {
    [key: string]: [number];
  };
  nodeIndex: {
    [key: string]: Node;
  };
  edgeIndex: {
    [key: string]: Road;
  };
}

// Entity Types
export interface Location {
  id: string;
  name: string;
  type: string;
  category?: string;
  population?: number;
  x: number;
  y: number;
  isCritical: boolean;
}

export interface RoadFull {
  id: number;
  fromLocationId: string;
  toLocationId: string;
  distance: number;
  capacity: number;
  condition?: number;
  isExisting: boolean;
  isTwoWay: boolean;
  constructionCost?: number;
}

export interface TransportRoute {
  id: string;
  type: string;
  dailyPassengers?: number;
  vehiclesAssigned?: number;
  capacityPerUnit: number;
}

export interface RouteStop {
  routeId: string;
  locationId: string;
  stopOrder: number;
}

export interface TrafficPeriodMultiplier {
  period: string;
  multiplier: number;
}

export interface TrafficFlow {
  id: number;
  roadId: number;
  period: string;
  flow: number;
}

export interface RoadMaintenance {
  roadId: number;
  priority?: number;
  estimatedCost?: number;
}

// Algorithm DTOs
export interface ShortestPathResultDto {
  fromNodeId: string;
  toNodeId: string;
  found: boolean;
  totalDistance: number;
  pathNodes: ShortestPathNodeDto[];
  pathRoads: ShortestPathRoadDto[];
}

export interface ShortestPathNodeDto {
  id: string;
  name: string;
  type: string;
  x?: number;
  y?: number;
  population?: number;
  isCritical: boolean;
}

export interface ShortestPathRoadDto {
  id: number;
  fromNodeId: string;
  toNodeId: string;
  distance: number;
  capacity: number;
  condition?: number;
  isExisting: boolean;
  constructionCost?: number;
}

export interface MstResultDto {
  connected: boolean;
  totalConstructionCost: number;
  totalNodes: number;
  selectedRoadCount: number;
  nodes: ShortestPathNodeDto[];
  selectedRoads: ShortestPathRoadDto[];
}

export interface MaintenancePlanningResultDto {
  budget: number;
  totalCost: number;
  remainingBudget: number;
  totalPriorityScore: number;
  selectedRoadCount: number;
  totalCandidateRoads: number;
  expectedConditionImprovement: number;
  selectedRoads: MaintenanceRoadDto[];
  notSelectedRoads: MaintenanceRoadDto[];
}

export interface MaintenanceRoadDto {
  roadId: number;
  fromLocation?: string;
  toLocation?: string;
  currentCondition?: number;
  estimatedCost?: number;
  priority?: number;
  expectedNewCondition?: number;
  reason: string;
}

export interface TransitSchedulingResultDto {
  totalVehicles: number;
  assignedVehicles: number;
  remainingVehicles: number;
  totalDemand: number;
  estimatedPassengersServed: number;
  coverageRatio: number;
  totalRoutes: number;
  activeRoutes: number;
  routeAllocations: RouteAllocationDto[];
}

export interface RouteAllocationDto {
  routeId: string;
  routeType: string;
  assignedVehicles: number;
  currentVehicles?: number;
  dailyPassengers?: number;
  stopCount: number;
  estimatedFrequencyMinutes?: number;
  estimatedServed: number;
  efficiencyScore: number;
  reason: string;
}

export interface TrafficSignalResultDto {
  period: string;
  roadsAnalyzed: number;
  intersectionsAnalyzed: number;
  intersectionsWithSignalRecommendations: number;
  signalRecommendations: number;
  totalCongestionScore: number;
  estimatedWaitTimeReductionPercent: number;
  signalTimings: SignalTimingDto[];
}

export interface SignalTimingDto {
  roadId: number;
  fromLocation?: string;
  toLocation?: string;
  currentFlow: number;
  roadCapacity: number;
  congestionRatio: number;
  priorityRank: number;
  recommendedGreenDurationSeconds: number;
  recommendedCycleTimeSeconds: number;
  reason: string;
}
