using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApi.Data;

/// <summary>
/// Factory for creating ApplicationDbContext instances during design-time operations (migrations)
/// Supports both MSSQL and PostgreSQL
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Determine which database provider to use
        // Default to PostgreSQL, but can be overridden via environment variable or args
        var databaseProvider = args?.FirstOrDefault() ?? 
                              Environment.GetEnvironmentVariable("DB_PROVIDER") ?? 
                              "PostgreSQL";

        var connectionString = databaseProvider.ToUpper() switch
        {
            "POSTGRESQL" => "Host=localhost;Port=5432;Database=myapidb;Username=postgres;Password=postgres;Include Error Detail=true",
            "MSSQL" => "Server=localhost;Database=MyApiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
            _ => throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}")
        };

        switch (databaseProvider.ToUpper())
        {
            case "POSTGRESQL":
                optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                });
                break;
            case "MSSQL":
                optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
                });
                break;
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
