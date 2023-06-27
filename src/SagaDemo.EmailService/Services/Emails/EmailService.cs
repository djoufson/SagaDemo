using System.Net.Mail;
using Polly;
using Polly.Retry;
using SagaDemo.EmailService.Configurations;
using SagaDemo.EmailService.Data;
using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Emails;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailDbContext _dbContext;
    private readonly SmtpClient _smtpClient;
    private readonly EmailSettings _emailSettings;
    private const int _delay = 1500;

    public EmailService(
        EmailDbContext dbContext,
        SmtpClient smtpClient,
        EmailSettings emailSettings,
        ILogger<EmailService> logger)
    {
        _dbContext = dbContext;
        _smtpClient = smtpClient;
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task<bool> SendAsync(Email email)
    {
        bool result = await Send(email.ToUserEmail, email.Object, email.Message);
        if(!result)
            return false;

        email.SentAt = DateTime.UtcNow;
        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<bool> Send(string emailAddress, string subject, string body)
    {
        var message = new MailMessage(
            from: _emailSettings.FromEmail,
            to: emailAddress,
            subject: subject,
            body: body
        );

        AsyncRetryPolicy policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => 
            {
                _logger.LogWarning("--> Email failed to send. Will retry after {Time}ms", attempt * _delay);
                return TimeSpan.FromMilliseconds(_delay * attempt);
            });

        PolicyResult result = await policy
            .ExecuteAndCaptureAsync(() => _smtpClient.SendMailAsync(message));
        bool success = result.FinalException is null;
        if(!success)
            _logger.LogError(result.FinalException, "--> Email definitely failed to send.");
        return success;
    }
}
