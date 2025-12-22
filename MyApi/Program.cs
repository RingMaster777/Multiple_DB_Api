using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Read database provider from configuration
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "MSSQL";
var connectionString = databaseProvider.ToUpper() switch
{
    "POSTGRESQL" => builder.Configuration.GetConnectionString("PostgreSQL"),
    "MSSQL" => builder.Configuration.GetConnectionString("MSSQL"),
    _ => throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}")
};

// Validate connection string
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException($"Connection string for {databaseProvider} is not configured");
}

// Configure DbContext with the selected database provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (databaseProvider.ToUpper())
    {
        case "POSTGRESQL":
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            });
            break;
        
        case "MSSQL":
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
            });
            break;
        
        default:
            throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
    }
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register services
builder.Services.AddScoped<IProductService, ProductService>();

// Add controllers
builder.Services.AddControllers();

// Add API documentation (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Multi-Database API", 
        Version = "v1",
        Description = $"API running on {databaseProvider} database"
    });
});

var app = builder.Build();

// Apply migrations automatically in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Applying migrations for {DatabaseProvider}...", databaseProvider);
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations");
            // Don't throw - allow app to start even if migrations fail
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Multi-Database API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", async (ApplicationDbContext dbContext) =>
{
    try
    {
        await dbContext.Database.CanConnectAsync();
        return Results.Ok(new { 
            status = "Healthy", 
            database = databaseProvider,
            timestamp = DateTime.UtcNow 
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Database connection failed",
            detail: ex.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable
        );
    }
}).WithName("HealthCheck").WithTags("Health");

app.Run();
