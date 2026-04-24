using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CairoTransportation.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(TransportationDbContext dbContext, IWebHostEnvironment environment)
    {
        if (await dbContext.Locations.AnyAsync())
        {
            return;
        }

        string scriptPath = Path.Combine(environment.ContentRootPath, "Data", "TablesData.sql");
        if (!File.Exists(scriptPath))
        {
            return;
        }

        string script = await File.ReadAllTextAsync(scriptPath);
        string[] statements = script.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
        foreach (string statement in statements)
        {
            if (!string.IsNullOrWhiteSpace(statement))
            {
                await dbContext.Database.ExecuteSqlRawAsync(statement + ";");
            }
        }

        await transaction.CommitAsync();
    }
}
