package com.softwave.transportsystem.model;

import com.softwave.transportsystem.model.Interfaces.AbstractEdge;

/**
 * Captures time-of-day traffic volumes for a specific road segment.
 * Data source: traffic_patterns.csv
 *
 * The linked road matches the "FromID-ToID" format used in the CSV
 * (e.g. "1-3", "F1-5"). Four time slots are recorded:
 * morningPeakVph – 07:00–09:00 rush hour (vehicles per hour)
 * afternoonVph – 12:00–14:00 off-peak
 * eveningPeakVph – 16:00–19:00 rush hour
 * nightVph – 22:00–05:00 low traffic
 */
public class TrafficPattern {

    private AbstractEdge road;
    private int morningPeakVph;
    private int afternoonVph;
    private int eveningPeakVph;
    private int nightVph;

    public TrafficPattern() {
    }

    public TrafficPattern(AbstractEdge road, int morningPeakVph, int afternoonVph,
            int eveningPeakVph, int nightVph) {
        this.road = road;
        this.morningPeakVph = morningPeakVph;
        this.afternoonVph = afternoonVph;
        this.eveningPeakVph = eveningPeakVph;
        this.nightVph = nightVph;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public AbstractEdge getRoad() {
        return road;
    }

    public void setRoad(AbstractEdge road) {
        this.road = road;
    }

    public String getRoadId() {
        return road.asRoadId();
    }

    public int getMorningPeakVph() {
        return morningPeakVph;
    }

    public void setMorningPeakVph(int v) {
        this.morningPeakVph = v;
    }

    public int getAfternoonVph() {
        return afternoonVph;
    }

    public void setAfternoonVph(int v) {
        this.afternoonVph = v;
    }

    public int getEveningPeakVph() {
        return eveningPeakVph;
    }

    public void setEveningPeakVph(int v) {
        this.eveningPeakVph = v;
    }

    public int getNightVph() {
        return nightVph;
    }

    public void setNightVph(int v) {
        this.nightVph = v;
    }
}
