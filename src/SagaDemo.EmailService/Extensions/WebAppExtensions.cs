using Microsoft.EntityFrameworkCore;
using SagaDemo.EmailService.Data;

namespace SagaDemo.EmailService.Extensions;

public static class WebAppExtensions
{
    public static WebApplication PrepDatabase(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            var scope = app.Services.CreateScope();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmailDbContext>();
            logger.LogInformation("--> Attempting to apply migrations . . .");
            dbContext.Database.Migrate();
            logger.LogInformation("--> Migrations applied successfully");
        }
        return app;
    }
}
