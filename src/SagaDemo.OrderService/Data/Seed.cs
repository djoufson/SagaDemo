using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Entities;

namespace SagaDemo.OrderService.Data;

internal static class Seed
{
    public static async void SeedData(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        using OrdersDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var any = await dbContext.Products.AnyAsync();
        if(any)
            return;

        Console.WriteLine("--> Seeding Data");
        await dbContext.Products.AddRangeAsync(new []
        {
            new Product{ Price = 2500, ProductName = "Product1", QuantityInStock = 25 },
            new Product{ Price = 100, ProductName = "Product2", QuantityInStock = 250 },
            new Product{ Price = 21500, ProductName = "Product3", QuantityInStock = 5 },
            new Product{ Price = 200, ProductName = "Product4", QuantityInStock = 125 },
        });

        await dbContext.SaveChangesAsync();
    }
}
