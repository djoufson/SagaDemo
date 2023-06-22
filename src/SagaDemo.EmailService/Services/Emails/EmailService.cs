using System.Net.Mail;
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
        Console.WriteLine(emailAddress);
        var message = new MailMessage(
            from: _emailSettings.FromEmail,
            to: emailAddress,
            // to: "djouflegran@gmail.com",
            subject: subject,
            body: body
        );

        try
        {
            await _smtpClient.SendMailAsync(message);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send the email");
            return false;
        }
    }
}
