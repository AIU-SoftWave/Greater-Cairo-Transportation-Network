using CairoTransportation.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=cairo_transportation.db";

builder.Services.AddDbContext<TransportationDbContext>(options =>
    options.UseSqlite(connectionString));

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
{
    TransportationDbContext dbContext = scope.ServiceProvider.GetRequiredService<TransportationDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapGet("/", () => "Hello World!");

app.Run();


