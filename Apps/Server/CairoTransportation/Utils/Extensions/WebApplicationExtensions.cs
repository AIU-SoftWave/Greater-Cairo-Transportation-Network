namespace CairoTransportation.Utils.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseProjectPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "CairoTransportation API v1"));
        }

        app.MapControllers();
        app.MapGet("/", () => Results.Redirect("/swagger"));

        return app;
    }
}
