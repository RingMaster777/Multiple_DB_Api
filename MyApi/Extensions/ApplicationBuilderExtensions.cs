using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace MyApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Multi-Database API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }

    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();            try
            {
                logger.LogInformation("Applying database migrations...");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully");
            }
            catch (Exception)
            {
                logger.LogError("An error occurred while applying migrations");
            }
        }
    }

    public static WebApplication MapHealthCheckEndpoint(this WebApplication app)
    {
        app.MapGet("/health", HealthCheckHandler.Handle)
            .WithName("HealthCheck")
            .WithTags("Health");

        return app;
    }
}

public static class HealthCheckHandler
{    public static async Task<IResult> Handle(ApplicationDbContext dbContext)
    {
        try
        {
            await dbContext.Database.CanConnectAsync();
            return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }
        catch (Exception)
        {
            return Results.Problem("Database connection failed", statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
