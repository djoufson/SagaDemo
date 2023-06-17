using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Entities;

namespace SagaDemo.OrderService.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
}
