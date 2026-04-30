"use client";

import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import type { AlgorithmResponse, ShortestPathResultDto } from "@/types";

interface RaceAnimationProps {
  responseA: AlgorithmResponse<ShortestPathResultDto> | null;
  responseB: AlgorithmResponse<ShortestPathResultDto> | null;
  algoA: string;
  algoB: string;
  isActive: boolean;
}

export default function RaceAnimation({
  responseA,
  responseB,
  algoA,
  algoB,
  isActive,
}: RaceAnimationProps) {
  const [phase, setPhase] = useState<"idle" | "racing" | "complete">("idle");
  const [progressA, setProgressA] = useState(0);
  const [progressB, setProgressB] = useState(0);

  const traceA = responseA?.trace;
  const traceB = responseB?.trace;

  // Calculate relative speeds based on execution time
  // Faster algorithm completes first
  const timeA = traceA?.executionTimeMs ?? 0;
  const timeB = traceB?.executionTimeMs ?? 0;
  const maxTime = Math.max(timeA, timeB, 1); // Avoid division by zero

  const durationA = (timeA / maxTime) * 2; // Scale to 2 seconds max
  const durationB = (timeB / maxTime) * 2;

  useEffect(() => {
    if (!isActive) {
      setPhase("idle");
      setProgressA(0);
      setProgressB(0);
      return;
    }

    if (phase === "idle") {
      setPhase("racing");
      
      // Start animations
      setTimeout(() => setProgressA(100), 100);
      setTimeout(() => setProgressB(100), 100);
      
      // Mark complete after max duration
      setTimeout(() => setPhase("complete"), 2500);
    }
  }, [isActive, phase]);

  if (!isActive || (!traceA && !traceB)) return null;

  const winner = timeA < timeB ? "A" : timeB < timeA ? "B" : "tie";

  return (
    <div className="mb-4 p-3 bg-gray-900 rounded-lg border border-gray-700">
      <h4 className="text-xs font-bold text-white uppercase mb-3 text-center">
        🏁 Algorithm Race
      </h4>

      {/* Algorithm A Progress */}
      <div className="mb-3">
        <div className="flex justify-between text-xs mb-1">
          <span className="text-blue-400 font-medium">{algoA}</span>
          <span className="text-gray-400">
            {traceA ? `${timeA.toFixed(2)}ms` : "—"}
          </span>
        </div>
        <div className="h-3 bg-gray-800 rounded-full overflow-hidden relative">
          <motion.div
            className="h-full bg-blue-500 rounded-full"
            initial={{ width: 0 }}
            animate={{ width: `${progressA}%` }}
            transition={{ 
              duration: durationA,
              ease: "easeOut",
              delay: 0.1
            }}
          />
          {winner === "A" && phase === "complete" && (
            <motion.span
              initial={{ scale: 0, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              className="absolute right-2 top-1/2 -translate-y-1/2 text-xs"
            >
              🏆
            </motion.span>
          )}
        </div>
        <div className="flex justify-between text-[10px] text-gray-500 mt-1">
          <span>Visited: {traceA?.visitedNodes ?? "—"}</span>
          <span>Expanded: {traceA?.expandedNodes ?? "—"}</span>
        </div>
      </div>

      {/* Algorithm B Progress */}
      <div>
        <div className="flex justify-between text-xs mb-1">
          <span className="text-green-400 font-medium">{algoB}</span>
          <span className="text-gray-400">
            {traceB ? `${timeB.toFixed(2)}ms` : "—"}
          </span>
        </div>
        <div className="h-3 bg-gray-800 rounded-full overflow-hidden relative">
          <motion.div
            className="h-full bg-green-500 rounded-full"
            initial={{ width: 0 }}
            animate={{ width: `${progressB}%` }}
            transition={{ 
              duration: durationB,
              ease: "easeOut",
              delay: 0.1
            }}
          />
          {winner === "B" && phase === "complete" && (
            <motion.span
              initial={{ scale: 0, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              className="absolute right-2 top-1/2 -translate-y-1/2 text-xs"
            >
              🏆
            </motion.span>
          )}
        </div>
        <div className="flex justify-between text-[10px] text-gray-500 mt-1">
          <span>Visited: {traceB?.visitedNodes ?? "—"}</span>
          <span>Expanded: {traceB?.expandedNodes ?? "—"}</span>
        </div>
      </div>

      {/* Race Status */}
      <AnimatePresence>
        {phase === "racing" && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="mt-3 text-center"
          >
            <span className="text-xs text-yellow-400 animate-pulse">
              ⚡ Racing...
            </span>
          </motion.div>
        )}
        {phase === "complete" && (
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            className="mt-3 text-center"
          >
            <span className="text-xs text-white">
              {winner === "tie" 
                ? "🤝 Tie!" 
                : `🎉 ${winner === "A" ? algoA : algoB} wins by ${Math.abs(timeA - timeB).toFixed(2)}ms!`}
            </span>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}
