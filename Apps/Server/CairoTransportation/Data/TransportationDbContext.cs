using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Data;

public class TransportationDbContext(DbContextOptions<TransportationDbContext> options)
    : DbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Road> Roads => Set<Road>();
    public DbSet<TrafficFlow> TrafficFlows => Set<TrafficFlow>();
    public DbSet<TransportRoute> TransportRoutes => Set<TransportRoute>();
    public DbSet<RouteStop> RouteStops => Set<RouteStop>();
    public DbSet<TransportDemand> TransportDemands => Set<TransportDemand>();
    public DbSet<RoadMaintenance> RoadMaintenances => Set<RoadMaintenance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("locations");

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Name).HasColumnName("name");
            entity.Property(x => x.Type).HasColumnName("type");
            entity.Property(x => x.Category).HasColumnName("category");
            entity.Property(x => x.Population).HasColumnName("population");
            entity.Property(x => x.X).HasColumnName("x");
            entity.Property(x => x.Y).HasColumnName("y");
            entity.Property(x => x.IsCritical).HasColumnName("is_critical").HasDefaultValue(false);
        });

        modelBuilder.Entity<Road>(entity =>
        {
            entity.ToTable("roads", table =>
            {
                table.HasCheckConstraint("chk_distance", "distance > 0");
                table.HasCheckConstraint("chk_capacity", "capacity > 0");
            });

            entity.HasIndex(x => x.FromLocationId).HasDatabaseName("idx_roads_from");
            entity.HasIndex(x => x.ToLocationId).HasDatabaseName("idx_roads_to");

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.FromLocationId).HasColumnName("from_location_id");
            entity.Property(x => x.ToLocationId).HasColumnName("to_location_id");
            entity.Property(x => x.Distance).HasColumnName("distance");
            entity.Property(x => x.Capacity).HasColumnName("capacity");
            entity.Property(x => x.Condition).HasColumnName("condition");
            entity.Property(x => x.IsExisting).HasColumnName("is_existing");
            entity.Property(x => x.ConstructionCost).HasColumnName("construction_cost");

            entity.HasOne(x => x.FromLocation)
                .WithMany(x => x.OutgoingRoads)
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_roads_from");

            entity.HasOne(x => x.ToLocation)
                .WithMany(x => x.IncomingRoads)
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_roads_to");
        });

        modelBuilder.Entity<TrafficFlow>(entity =>
        {
            entity.ToTable("traffic_flow", table => table.HasCheckConstraint("chk_flow", "flow >= 0"));

            entity.HasIndex(x => x.RoadId).HasDatabaseName("idx_traffic_road");
            entity.HasIndex(x => x.Period).HasDatabaseName("idx_traffic_period");

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.RoadId).HasColumnName("road_id");
            entity.Property(x => x.Period).HasColumnName("period");
            entity.Property(x => x.Flow).HasColumnName("flow");

            entity.HasOne(x => x.Road)
                .WithMany(x => x.TrafficFlows)
                .HasForeignKey(x => x.RoadId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_traffic_road");
        });

        modelBuilder.Entity<TransportRoute>(entity =>
        {
            entity.ToTable("transport_routes");

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Type).HasColumnName("type");
            entity.Property(x => x.DailyPassengers).HasColumnName("daily_passengers");
            entity.Property(x => x.VehiclesAssigned).HasColumnName("vehicles_assigned");
        });

        modelBuilder.Entity<RouteStop>(entity =>
        {
            entity.ToTable("route_stops");

            entity.HasKey(x => new { x.RouteId, x.LocationId });
            entity.HasIndex(x => new { x.RouteId, x.StopOrder }).HasDatabaseName("idx_route_stops_order");

            entity.Property(x => x.RouteId).HasColumnName("route_id");
            entity.Property(x => x.LocationId).HasColumnName("location_id");
            entity.Property(x => x.StopOrder).HasColumnName("stop_order");

            entity.HasOne(x => x.Route)
                .WithMany(x => x.RouteStops)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_route_stops_route");

            entity.HasOne(x => x.Location)
                .WithMany(x => x.RouteStops)
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_route_stops_location");
        });

        modelBuilder.Entity<TransportDemand>(entity =>
        {
            entity.ToTable("transport_demand");

            entity.HasIndex(x => x.FromLocationId).HasDatabaseName("idx_demand_from");
            entity.HasIndex(x => x.ToLocationId).HasDatabaseName("idx_demand_to");

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.FromLocationId).HasColumnName("from_location_id");
            entity.Property(x => x.ToLocationId).HasColumnName("to_location_id");
            entity.Property(x => x.DailyPassengers).HasColumnName("daily_passengers");

            entity.HasOne(x => x.FromLocation)
                .WithMany(x => x.OriginDemands)
                .HasForeignKey(x => x.FromLocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_demand_from");

            entity.HasOne(x => x.ToLocation)
                .WithMany(x => x.DestinationDemands)
                .HasForeignKey(x => x.ToLocationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_demand_to");
        });

        modelBuilder.Entity<RoadMaintenance>(entity =>
        {
            entity.ToTable("road_maintenance");

            entity.Property(x => x.RoadId).HasColumnName("road_id");
            entity.Property(x => x.Priority).HasColumnName("priority");
            entity.Property(x => x.EstimatedCost).HasColumnName("estimated_cost");

            entity.HasOne(x => x.Road)
                .WithOne(x => x.Maintenance)
                .HasForeignKey<RoadMaintenance>(x => x.RoadId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
