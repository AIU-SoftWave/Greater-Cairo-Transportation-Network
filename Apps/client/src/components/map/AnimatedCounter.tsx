"use client";

import { useEffect, useState, useRef } from "react";
import { motion, useSpring } from "framer-motion";

interface AnimatedCounterProps {
  value: number;
  duration?: number;
  delay?: number;
  decimals?: number;
  suffix?: string;
  className?: string;
  isActive: boolean;
}

export default function AnimatedCounter({
  value,
  duration = 1,
  delay = 0,
  decimals = 0,
  suffix = "",
  className = "",
  isActive,
}: AnimatedCounterProps) {
  const [displayValue, setDisplayValue] = useState(0);
  const hasAnimated = useRef(false);

  const spring = useSpring(0, {
    stiffness: 50,
    damping: 20,
    duration: duration * 1000,
  });

  useEffect(() => {
    if (!isActive) {
      setDisplayValue(0);
      hasAnimated.current = false;
      return;
    }

    if (hasAnimated.current) return;

    const timeout = setTimeout(() => {
      spring.set(value);
      hasAnimated.current = true;
    }, delay * 1000);

    const unsubscribe = spring.on("change", (latest: number) => {
      setDisplayValue(latest);
    });

    return () => {
      clearTimeout(timeout);
      unsubscribe();
    };
  }, [isActive, value, spring, delay]);

  const formatted =
    decimals > 0
      ? displayValue.toFixed(decimals)
      : Math.round(displayValue).toString();

  return (
    <motion.span
      className={className}
      initial={{ opacity: 0, y: 10 }}
      animate={isActive ? { opacity: 1, y: 0 } : { opacity: 0, y: 10 }}
      transition={{ duration: 0.3, delay }}
    >
      {formatted}
      {suffix}
    </motion.span>
  );
}
