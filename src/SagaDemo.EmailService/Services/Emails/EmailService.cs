using SagaDemo.EmailService.Data;
using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Emails;

public class EmailService : IEmailService
{
    private readonly EmailDbContext _dbContext;

    public EmailService(EmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> SendAsync(Email email)
    {
        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
