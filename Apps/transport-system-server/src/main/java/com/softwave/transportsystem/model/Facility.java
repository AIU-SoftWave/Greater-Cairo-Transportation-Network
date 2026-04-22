package com.softwave.transportsystem.model;

/**
 * Represents an important facility in Cairo (airport, hospital, university, etc.).
 * Data source: facilities.csv
 *
 * Fields:
 *   id        – String identifier prefixed with "F" (e.g. "F1", "F9")
 *   name      – Full facility name
 *   type      – Category: Airport, Transit Hub, Education, Tourism, Sports,
 *               Business, Commercial, or Medical
 *   longitude – WGS-84 longitude
 *   latitude  – WGS-84 latitude
 */
public class Facility {

    private String id;
    private String name;
    private String type;
    private double longitude;
    private double latitude;

    public Facility() {}

    public Facility(String id, String name, String type,
                    double longitude, double latitude) {
        this.id        = id;
        this.name      = name;
        this.type      = type;
        this.longitude = longitude;
        this.latitude  = latitude;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public String getId()                   { return id; }
    public void setId(String id)            { this.id = id; }

    public String getName()                 { return name; }
    public void setName(String name)        { this.name = name; }

    public String getType()                 { return type; }
    public void setType(String type)        { this.type = type; }

    public double getLongitude()            { return longitude; }
    public void setLongitude(double lng)    { this.longitude = lng; }

    public double getLatitude()             { return latitude; }
    public void setLatitude(double lat)     { this.latitude = lat; }
}
