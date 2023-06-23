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
        User rosane = new()
        {
            Email = "temgouarosane@gmail.com",
            Name = "Rosane",
            Password = "RosanePassword 1"
        };

        User djouf = new()
        {
            Email = "djouflegran@gmail.com",
            Name = "Djoufson",
            Password = "DjoufPassword 1"
        };

        User michel = new()
        {
            Email = "michelbosseck@gmail.com",
            Name = "Michel",
            Password = "MichelPassword 1"
        };

        await dbContext.Users.AddRangeAsync(rosane, djouf, michel);
        await client.PublishUserRegisteredAsync(new UserRegistered()
        {
            Id = rosane.Id,
            Name = rosane.Name,
            Email = rosane.Email
        });
        await client.PublishUserRegisteredAsync(new UserRegistered()
        {
            Id = djouf.Id,
            Name = djouf.Name,
            Email = djouf.Email
        });
        await client.PublishUserRegisteredAsync(new UserRegistered()
        {
            Id = michel.Id,
            Name = michel.Name,
            Email = michel.Email
        });
        await dbContext.SaveChangesAsync();
    }
}
