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
  data: T; // refine later per algorithm
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
