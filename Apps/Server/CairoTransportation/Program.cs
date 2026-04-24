using CairoTransportation.Data;
using CairoTransportation.Services;
using CairoTransportation.Services.Graph;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TransportationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IRoadService, RoadService>();
builder.Services.AddScoped<ITrafficService, TrafficService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IGraphService, GraphService>();

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
{
    TransportationDbContext dbContext = scope.ServiceProvider.GetRequiredService<TransportationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(dbContext, app.Environment);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "CairoTransportation API v1"));
}

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();


