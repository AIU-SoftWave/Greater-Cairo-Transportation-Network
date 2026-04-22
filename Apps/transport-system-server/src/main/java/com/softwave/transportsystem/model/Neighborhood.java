package com.softwave.transportsystem.model;

/**
 * Represents a neighborhood or district in the Greater Cairo area.
 * Data source: nodes.csv
 *
 * Fields:
 *   id         – Unique integer identifier (1-15)
 *   name       – Human-readable district name (e.g. "Maadi")
 *   population – Estimated resident population
 *   type       – Land-use classification: Residential, Mixed, Business,
 *                Industrial, or Government
 *   longitude  – WGS-84 longitude (x-axis on Cairo maps)
 *   latitude   – WGS-84 latitude  (y-axis on Cairo maps)
 */
public class Neighborhood {

    private int id;
    private String name;
    private int population;
    private String type;
    private double longitude;
    private double latitude;

    public Neighborhood() {}

    public Neighborhood(int id, String name, int population, String type,
                        double longitude, double latitude) {
        this.id        = id;
        this.name      = name;
        this.population = population;
        this.type      = type;
        this.longitude = longitude;
        this.latitude  = latitude;
    }

    // ── Getters & Setters ──────────────────────────────────────────────────────

    public int getId()                    { return id; }
    public void setId(int id)             { this.id = id; }

    public String getName()               { return name; }
    public void setName(String name)      { this.name = name; }

    public int getPopulation()            { return population; }
    public void setPopulation(int p)      { this.population = p; }

    public String getType()               { return type; }
    public void setType(String type)      { this.type = type; }

    public double getLongitude()          { return longitude; }
    public void setLongitude(double lng)  { this.longitude = lng; }

    public double getLatitude()           { return latitude; }
    public void setLatitude(double lat)   { this.latitude = lat; }
}
