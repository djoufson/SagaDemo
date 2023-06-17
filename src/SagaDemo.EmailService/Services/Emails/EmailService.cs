using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Emails;

public class EmailService : IEmailService
{
    public Task<bool> SendAsync(Email email)
    {
        return Task.FromResult(true);
    }
}
