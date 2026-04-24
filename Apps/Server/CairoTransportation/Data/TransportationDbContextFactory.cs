using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CairoTransportation.Data;

public class TransportationDbContextFactory : IDesignTimeDbContextFactory<TransportationDbContext>
{
    public TransportationDbContext CreateDbContext(string[] args)
    {
        const string connectionString = "Data Source=cairo_transportation.db";

        DbContextOptionsBuilder<TransportationDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite(connectionString);

        return new TransportationDbContext(optionsBuilder.Options);
    }
}
