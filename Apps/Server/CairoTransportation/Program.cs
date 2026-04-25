using CairoTransportation.Data;
using CairoTransportation.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services
    .AddProjectInfrastructure(builder.Configuration)
    .AddProjectApplicationServices();

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
{
    TransportationDbContext dbContext = scope.ServiceProvider.GetRequiredService<TransportationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(dbContext, app.Environment);
}

app.UseProjectPipeline();

app.Run();


