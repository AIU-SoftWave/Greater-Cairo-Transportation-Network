package com.softwave.transportsystem.traffic.model;

import java.util.Locale;

/**
 * Supported traffic time slots used by congestion-aware routing.
 */
public enum TrafficTimeSlot {
    MORNING,
    AFTERNOON,
    EVENING,
    NIGHT;

    public static TrafficTimeSlot from(String rawValue) {
        if (rawValue == null || rawValue.isBlank()) {
            throw new IllegalArgumentException(
                    "timeSlot is required. Supported values: MORNING, AFTERNOON, EVENING, NIGHT.");
        }

        try {
            return TrafficTimeSlot.valueOf(rawValue.trim().toUpperCase(Locale.ROOT));
        } catch (IllegalArgumentException exception) {
            throw new IllegalArgumentException(
                    "Unsupported timeSlot '" + rawValue
                            + "'. Supported values: MORNING, AFTERNOON, EVENING, NIGHT.");
        }
    }
}
