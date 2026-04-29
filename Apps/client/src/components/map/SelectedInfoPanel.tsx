"use client";

import type { Node, Road } from "@/types";

interface NodeWithSignalData extends Node {
  signalData?: {
    intersectionName: string;
    cycleTimeSeconds: number;
    signals: Array<{
      from: string;
      to: string;
      congestionPercent: number;
      priority: number;
      greenTimeSeconds: number;
    }>;
    maxCongestion: number;
  };
}

interface SelectedInfoPanelProps {
  selectedNode: Node | null;
  selectedRoad: Road | null;
  nodeLookup: Record<string, Node>;
}

export default function SelectedInfoPanel({
  selectedNode,
  selectedRoad,
  nodeLookup,
}: SelectedInfoPanelProps) {
  if (!selectedNode && !selectedRoad) return null;

  const nodeWithSignal = selectedNode as NodeWithSignalData | null;

  return (
    <div className="mb-3 rounded-md bg-blue-50 p-3 text-xs">
      <p className="mb-2 font-semibold text-gray-700">
        {selectedNode ? "Node Info" : "Road Info"}
      </p>
      {selectedNode && (
        <div className="space-y-1">
          <div>
            <span className="text-gray-500">Name:</span>
            <span className="ml-1 font-medium text-black">
              {selectedNode.name}
            </span>
          </div>
          <div>
            <span className="text-gray-500">ID:</span>
            <span className="ml-1 font-medium text-black">
              {selectedNode.id}
            </span>
          </div>
          <div>
            <span className="text-gray-500">Type:</span>
            <span className="ml-1 font-medium text-black">
              {selectedNode.type}
            </span>
          </div>
          {selectedNode.population && (
            <div>
              <span className="text-gray-500">Population:</span>
              <span className="ml-1 font-medium text-black">
                {selectedNode.population.toLocaleString()}
              </span>
            </div>
          )}
          <div>
            <span className="text-gray-500">Coordinates:</span>
            <span className="ml-1 font-medium text-black">
              ({selectedNode.y.toFixed(4)}, {selectedNode.x.toFixed(4)})
            </span>
          </div>
          <div>
            <span className="text-gray-500">Critical:</span>
            <span className="ml-1 font-medium text-black">
              {selectedNode.isCritical ? "Yes" : "No"}
            </span>
          </div>
          {/* Signal Optimization Data (when available) */}
          {nodeWithSignal?.signalData && (
            <>
              <div className="pt-2 border-t border-gray-200 mt-2">
                <span className="font-semibold text-gray-700">
                  Signal Optimization:
                </span>
              </div>
              <div>
                <span className="text-gray-500">Intersection:</span>
                <span className="ml-1 font-medium text-black">
                  {nodeWithSignal.signalData.intersectionName}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Cycle Time:</span>
                <span className="ml-1 font-medium text-black">
                  {nodeWithSignal.signalData.cycleTimeSeconds}s
                </span>
              </div>
              <div>
                <span className="text-gray-500">Roads Optimized:</span>
                <span className="ml-1 font-medium text-black">
                  {nodeWithSignal.signalData.signals.length}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Max Congestion:</span>
                <span className="ml-1 font-medium text-black">
                  {(nodeWithSignal.signalData.maxCongestion * 100).toFixed(0)}%
                </span>
              </div>
              <div className="mt-2">
                <span className="font-semibold text-gray-600 text-xs">
                  Signal Phases:
                </span>
                {nodeWithSignal.signalData.signals.map((signal, idx) => (
                  <div
                    key={idx}
                    className="mt-1 pl-2 border-l-2 border-blue-300"
                  >
                    <p className="text-xs font-medium text-black">
                      {signal.from} → {signal.to}
                    </p>
                    <p className="text-xs text-gray-600 text-black">
                      Green: {signal.greenTimeSeconds}s (Cycle:{" "}
                      {nodeWithSignal.signalData!.cycleTimeSeconds}s)
                    </p>
                    <p className="text-xs text-gray-600 text-black">
                      Congestion: {signal.congestionPercent.toFixed(0)}%
                      (Priority #{signal.priority})
                    </p>
                  </div>
                ))}
              </div>
            </>
          )}
        </div>
      )}
      {selectedRoad && (
        <div className="space-y-1">
          <div>
            <span className="text-gray-500">From:</span>
            <span className="ml-1 font-medium text-black">
              {nodeLookup[selectedRoad.fromNodeId]?.name ||
                selectedRoad.fromNodeId}
            </span>
          </div>
          <div>
            <span className="text-gray-500">To:</span>
            <span className="ml-1 font-medium text-black">
              {nodeLookup[selectedRoad.toNodeId]?.name ||
                selectedRoad.toNodeId}
            </span>
          </div>
          <div>
            <span className="text-gray-500">Distance:</span>
            <span className="ml-1 font-medium text-black">
              {selectedRoad.distance.toFixed(2)} km
            </span>
          </div>
          <div>
            <span className="text-gray-500">Capacity:</span>
            <span className="ml-1 font-medium text-black">
              {selectedRoad.capacity.toLocaleString()}
            </span>
          </div>
          {selectedRoad.condition && (
            <div>
              <span className="text-gray-500">Condition:</span>
              <span className="ml-1 font-medium text-black">
                {selectedRoad.condition}/10
              </span>
            </div>
          )}
          <div>
            <span className="text-gray-500">Existing:</span>
            <span className="ml-1 font-medium text-black">
              {selectedRoad.isExisting ? "Yes" : "No"}
            </span>
          </div>
          {selectedRoad.constructionCost && (
            <div>
              <span className="text-gray-500">Construction Cost:</span>
              <span className="ml-1 font-medium text-black">
                ${selectedRoad.constructionCost.toFixed(0)}
              </span>
            </div>
          )}
          {selectedRoad.maintenancePriority && (
            <div className="pt-2 border-t border-gray-200">
              <span className="font-semibold text-gray-700">
                Maintenance Info:
              </span>
            </div>
          )}
          {selectedRoad.maintenancePriority && (
            <div>
              <span className="text-gray-500">Priority:</span>
              <span className="ml-1 font-medium text-black">
                {selectedRoad.maintenancePriority}/10
              </span>
            </div>
          )}
          {selectedRoad.maintenanceCost && (
            <div>
              <span className="text-gray-500">Est. Cost:</span>
              <span className="ml-1 font-medium text-black">
                ${selectedRoad.maintenanceCost.toFixed(0)}
              </span>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
