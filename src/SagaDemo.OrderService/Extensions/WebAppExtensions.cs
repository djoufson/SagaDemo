using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Data;

namespace SagaDemo.OrderService.Extensions;

public static class WebAppExtensions
{
    public static WebApplication PrepDatabase(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            var scope = app.Services.CreateScope();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            logger.LogInformation("--> Attempting to apply migrations . . .");
            dbContext.Database.Migrate();
        }
        return app;
    }
}
