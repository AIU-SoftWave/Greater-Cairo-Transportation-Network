package com.softwave.transportsystem.service;

import com.softwave.transportsystem.model.TrafficPattern;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

/**
 * Business logic for time-of-day traffic patterns.
 *
 * Provides:
 *  - Listing all traffic patterns
 *  - Looking up the pattern for a specific road
 *  - Identifying the most congested roads during peak hours
 *
 * Time slots used in the data:
 *   morning   – 07:00-09:00
 *   afternoon – 12:00-14:00
 *   evening   – 16:00-19:00
 *   night     – 22:00-05:00
 */
@Service
public class TrafficService {

    private final DataLoaderService data;

    public TrafficService(DataLoaderService data) {
        this.data = data;
    }

    /** Returns all traffic patterns. */
    public List<TrafficPattern> getAll() {
        return data.getTrafficPatterns();
    }

    /**
     * Returns the traffic pattern for a road identified by its "FromID-ToID"
     * key (e.g. "1-3" or "F1-5").
     */
    public Optional<TrafficPattern> getByRoadId(String roadId) {
        return data.getTrafficPatterns().stream()
            .filter(tp -> tp.getRoadId().equals(roadId))
            .findFirst();
    }

    /**
     * Returns roads whose morning-peak volume exceeds the given threshold,
     * sorted by morning-peak volume descending (most congested first).
     */
    public List<TrafficPattern> getMorningCongestion(int minVph) {
        return data.getTrafficPatterns().stream()
            .filter(tp -> tp.getMorningPeakVph() >= minVph)
            .sorted((a, b) -> Integer.compare(b.getMorningPeakVph(), a.getMorningPeakVph()))
            .toList();
    }

    /**
     * Returns roads whose evening-peak volume exceeds the given threshold,
     * sorted by evening-peak volume descending.
     */
    public List<TrafficPattern> getEveningCongestion(int minVph) {
        return data.getTrafficPatterns().stream()
            .filter(tp -> tp.getEveningPeakVph() >= minVph)
            .sorted((a, b) -> Integer.compare(b.getEveningPeakVph(), a.getEveningPeakVph()))
            .toList();
    }
}
