package com.softwave.transportsystem.traffic.model;

import com.softwave.transportsystem.road.model.Road;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToOne;
import jakarta.persistence.Table;

/**
 * Captures time-of-day traffic volumes for a specific road segment.
 */
@Entity
@Table(name = "traffic_patterns")
public class TrafficPattern {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @OneToOne(fetch = FetchType.EAGER, optional = false)
    @JoinColumn(name = "road_id", nullable = false, unique = true)
    private Road road;

    private int morningPeakVph;
    private int afternoonVph;
    private int eveningPeakVph;
    private int nightVph;

    public TrafficPattern() {
    }

    public TrafficPattern(Road road, int morningPeakVph, int afternoonVph,
            int eveningPeakVph, int nightVph) {
        this.road = road;
        this.morningPeakVph = morningPeakVph;
        this.afternoonVph = afternoonVph;
        this.eveningPeakVph = eveningPeakVph;
        this.nightVph = nightVph;
    }

    public Long getId() {
        return id;
    }

    public Road getRoad() {
        return road;
    }

    public void setRoad(Road road) {
        this.road = road;
    }

    public String getRoadId() {
        return road.asRoadId();
    }

    public int getMorningPeakVph() {
        return morningPeakVph;
    }

    public void setMorningPeakVph(int morningPeakVph) {
        this.morningPeakVph = morningPeakVph;
    }

    public int getAfternoonVph() {
        return afternoonVph;
    }

    public void setAfternoonVph(int afternoonVph) {
        this.afternoonVph = afternoonVph;
    }

    public int getEveningPeakVph() {
        return eveningPeakVph;
    }

    public void setEveningPeakVph(int eveningPeakVph) {
        this.eveningPeakVph = eveningPeakVph;
    }

    public int getNightVph() {
        return nightVph;
    }

    public void setNightVph(int nightVph) {
        this.nightVph = nightVph;
    }
}
