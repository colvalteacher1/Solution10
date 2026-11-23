using Microsoft.EntityFrameworkCore;
using MyApp.ProductsApi.Models;

namespace MyApp.ProductsApi.Data;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(product => product.Price)
                .HasPrecision(18, 2);
        });
    }
}
