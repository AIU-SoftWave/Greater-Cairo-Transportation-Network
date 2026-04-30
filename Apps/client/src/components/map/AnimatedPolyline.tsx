"use client";

import { useEffect, useState, useRef } from "react";
import { Polyline } from "react-leaflet";
import type { LatLngExpression } from "leaflet";

interface AnimatedPolylineProps {
  positions: LatLngExpression[];
  color: string;
  weight?: number;
  opacity?: number;
  dashArray?: string;
  duration?: number; // seconds
  enabled?: boolean;
  delay?: number;
  onComplete?: () => void;
}

export default function AnimatedPolyline({
  positions,
  color,
  weight = 5,
  opacity = 0.9,
  dashArray,
  duration = 2,
  enabled = true,
  delay = 0,
  onComplete,
}: AnimatedPolylineProps) {
  const [visiblePositions, setVisiblePositions] = useState<LatLngExpression[]>([]);
  const animationRef = useRef<number | null>(null);
  const startTimeRef = useRef<number | null>(null);

  useEffect(() => {
    if (!enabled) {
      setVisiblePositions(positions);
      return;
    }

    // Reset and start animation
    setVisiblePositions([]);
    
    const startDelay = setTimeout(() => {
      startTimeRef.current = Date.now();
      
      const animate = () => {
        const elapsed = (Date.now() - (startTimeRef.current || 0)) / 1000;
        const progress = Math.min(elapsed / duration, 1);
        
        // Calculate how many points to show
        const totalPoints = positions.length;
        const pointsToShow = Math.max(1, Math.floor(progress * totalPoints));
        
        setVisiblePositions(positions.slice(0, pointsToShow));
        
        if (progress < 1) {
          animationRef.current = requestAnimationFrame(animate);
        } else {
          setVisiblePositions(positions);
          onComplete?.();
        }
      };
      
      animationRef.current = requestAnimationFrame(animate);
    }, delay * 1000);

    return () => {
      clearTimeout(startDelay);
      if (animationRef.current) {
        cancelAnimationFrame(animationRef.current);
      }
    };
  }, [positions, duration, enabled, delay, onComplete]);

  if (visiblePositions.length < 2) return null;

  return (
    <Polyline
      positions={visiblePositions}
      pathOptions={{
        color,
        weight,
        opacity,
        dashArray,
        lineCap: "round",
        lineJoin: "round",
      }}
    />
  );
}
