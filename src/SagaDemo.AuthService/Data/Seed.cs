using Microsoft.EntityFrameworkCore;
using SagaDemo.AuthService.Models;

namespace SagaDemo.AuthService.Data;

internal static class Seed
{
    public static async void SeedData(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        using AuthDbContext dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var any = await dbContext.Users.AnyAsync();
        if (any)
            return;

        Console.WriteLine("--> Seeding Data");
        await dbContext.Users.AddAsync(
            new User 
            {
                Email = "djoufson@email.com",
                Name = "Djoufson",
                Password = "DjoufsonPassword 1"
            });

        await dbContext.SaveChangesAsync();
    }
}
