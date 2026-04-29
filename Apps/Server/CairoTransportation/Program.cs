using CairoTransportation.Data;
using CairoTransportation.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
}
builder.Services
    .AddProjectInfrastructure(builder.Configuration)
    .AddProjectApplicationServices();
builder.Services.AddCors(options => options.AddPolicy("AllowFrontend", policy =>
    policy.WithOrigins(
        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new string[0]
    )
    .AllowAnyMethod()
    .AllowAnyHeader()));
WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
{
    TransportationDbContext dbContext = scope.ServiceProvider.GetRequiredService<TransportationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(dbContext, app.Environment);
}

// CORS must be before any custom middleware or endpoints
app.UseCors("AllowFrontend");
app.UseProjectPipeline();

app.Run();


