using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CairoTransportation.Data;

public class TransportationDbContextFactory : IDesignTimeDbContextFactory<TransportationDbContext>
{
    public TransportationDbContext CreateDbContext(string[] args)
    {
        string connectionString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false).AddJsonFile("appsettings.Development.json", optional: true).AddEnvironmentVariables().Build().GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");

        DbContextOptionsBuilder<TransportationDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite(connectionString);

        return new TransportationDbContext(optionsBuilder.Options);
    }
}
