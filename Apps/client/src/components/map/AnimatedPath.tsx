"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";

interface AnimatedPathProps {
  positions: [number, number][];
  color: string;
  delay?: number;
  duration?: number;
  onComplete?: () => void;
  isActive: boolean;
}

export default function AnimatedPath({
  positions,
  color,
  delay = 0,
  duration = 1,
  onComplete,
  isActive,
}: AnimatedPathProps) {
  const [progress, setProgress] = useState(0);

  useEffect(() => {
    if (!isActive) {
      setProgress(0);
      return;
    }

    const timeout = setTimeout(() => {
      setProgress(1);
    }, delay * 1000);

    const completeTimeout = setTimeout(() => {
      onComplete?.();
    }, (delay + duration) * 1000);

    return () => {
      clearTimeout(timeout);
      clearTimeout(completeTimeout);
    };
  }, [isActive, delay, duration, onComplete]);

  if (positions.length < 2) return null;

  // Convert lat/lng to a simple path representation
  // This is a simplified version - in practice you'd use a proper projection
  const pathD = positions
    .map((pos, i) => `${i === 0 ? "M" : "L"} ${pos[1]} ${pos[0]}`)
    .join(" ");

  return (
    <svg
      style={{
        position: "absolute",
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
        pointerEvents: "none",
        zIndex: 1000,
      }}
    >
      <motion.path
        d={pathD}
        stroke={color}
        strokeWidth={4}
        fill="none"
        initial={{ pathLength: 0, opacity: 0 }}
        animate={{
          pathLength: isActive ? progress : 0,
          opacity: isActive ? 1 : 0,
        }}
        transition={{
          pathLength: {
            duration: duration,
            ease: "easeInOut",
          },
          opacity: { duration: 0.2 },
        }}
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}
