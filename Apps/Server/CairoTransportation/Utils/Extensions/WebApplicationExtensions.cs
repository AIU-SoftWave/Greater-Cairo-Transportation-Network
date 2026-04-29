namespace CairoTransportation.Utils.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseProjectPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapControllers();
        app.MapGet("/", () => Results.Redirect("/openapi/v1.json"));

        return app;
    }
}
