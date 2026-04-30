"use client";

import { motion, AnimatePresence } from "framer-motion";

interface Node {
  id: string;
  x: number;
  y: number;
}

interface AnimatedVisitedNodesProps {
  nodes: Node[];
  color: string;
  delay?: number;
  duration?: number;
  maxNodes?: number;
  isActive: boolean;
}

export default function AnimatedVisitedNodes({
  nodes,
  color,
  delay = 0,
  duration = 1,
  maxNodes = 50,
  isActive,
}: AnimatedVisitedNodesProps) {
  // Limit nodes to animate for performance
  const nodesToAnimate = nodes.slice(0, maxNodes);
  const stepDelay = duration / nodesToAnimate.length;

  if (!isActive || nodesToAnimate.length === 0) return null;

  return (
    <div
      style={{
        position: "absolute",
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
        pointerEvents: "none",
        zIndex: 999,
      }}
    >
      <AnimatePresence>
        {nodesToAnimate.map((node, index) => (
          <motion.div
            key={`${node.id}-${color}`}
            initial={{ scale: 0, opacity: 0 }}
            animate={{ scale: [0, 1.5, 1], opacity: [0, 0.8, 0.3] }}
            exit={{ opacity: 0, scale: 0 }}
            transition={{
              duration: 0.5,
              delay: delay + index * stepDelay,
              ease: "easeOut",
            }}
            style={{
              position: "absolute",
              left: `${((node.x + 180) / 360) * 100}%`,
              top: `${((90 - node.y) / 180) * 100}%`,
              width: "12px",
              height: "12px",
              borderRadius: "50%",
              backgroundColor: color,
              transform: "translate(-50%, -50%)",
              boxShadow: `0 0 10px ${color}`,
            }}
          />
        ))}
      </AnimatePresence>
    </div>
  );
}
