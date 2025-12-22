using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data;

/// <summary>
/// Database-agnostic ApplicationDbContext
/// Works with both MSSQL and PostgreSQL without modification
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity (database-agnostic)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Price)
                .HasPrecision(18, 2); // Works for both MSSQL and PostgreSQL
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Create index for better query performance
            entity.HasIndex(e => e.Name);
        });

        // Seed initial data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Sample Product 1",
                Description = "This is a sample product",
                Price = 29.99m,
                Stock = 100,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Sample Product 2",
                Description = "Another sample product",
                Price = 49.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
