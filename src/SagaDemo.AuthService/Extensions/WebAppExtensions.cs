using Microsoft.EntityFrameworkCore;
using SagaDemo.AuthService.Data;

namespace SagaDemo.AuthService.Extensions;

public static class WebAppExtensions
{
    public static WebApplication PrepDatabase(this WebApplication app)
    {
        if(!app.Environment.IsDevelopment())
        {
            var scope = app.Services.CreateScope();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            logger.LogInformation("--> Attempting to apply migrations . . .");
            dbContext.Database.Migrate();
        }
        return app;
    }
}
