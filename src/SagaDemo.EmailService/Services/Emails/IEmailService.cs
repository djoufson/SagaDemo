using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Emails;

public interface IEmailService
{
    Task<bool> SendAsync(Email email);
}
