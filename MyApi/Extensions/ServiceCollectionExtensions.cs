using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Services;

namespace MyApi.Extensions;

public static class ServiceCollectionExtensions
{    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var databaseProvider = configuration["DatabaseProvider"] ?? "MSSQL";
        var connectionString = GetConnectionString(configuration, databaseProvider);

        services.AddDbContext<ApplicationDbContext>(options =>
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

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped<IProductService, ProductService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Multi-Database API", Version = "v1" }));

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration, string databaseProvider)
    {
        var connectionString = databaseProvider.ToUpper() switch
        {
            "POSTGRESQL" => configuration.GetConnectionString("PostgreSQL"),
            "MSSQL" => configuration.GetConnectionString("MSSQL"),
            _ => throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}")
        };

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string for {databaseProvider} is not configured");
        }

        return connectionString;
    }
}
