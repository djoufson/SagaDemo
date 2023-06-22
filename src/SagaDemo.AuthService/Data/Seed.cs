using Microsoft.EntityFrameworkCore;
using SagaDemo.AuthService.Contracts;
using SagaDemo.AuthService.Models;
using SagaDemo.AuthService.Services.RabbitMq;

namespace SagaDemo.AuthService.Data;

internal static class Seed
{
    public static async void SeedData(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        using AuthDbContext dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        IBroadcastClient client = scope.ServiceProvider.GetRequiredService<IBroadcastClient>();
        var any = await dbContext.Users.AnyAsync();
        if (any)
            return;

        Console.WriteLine("--> Seeding Data");
        User user = new()
        {
            Email = "temgouarosane@gmail.com",
            Name = "Rosane",
            Password = "RosanePassword 1"
        };
        await dbContext.Users.AddAsync(user);
        await client.PublishUserRegisteredAsync(new UserRegistered()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        });
        await dbContext.SaveChangesAsync();
    }
}
