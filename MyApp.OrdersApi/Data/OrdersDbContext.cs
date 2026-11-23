using Microsoft.EntityFrameworkCore;
using MyApp.OrdersApi.Models;

namespace MyApp.OrdersApi.Data;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(order => order.TotalPrice)
                .HasPrecision(18, 2);
        });
    }
}
