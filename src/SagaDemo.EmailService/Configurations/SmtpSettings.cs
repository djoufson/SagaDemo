namespace SagaDemo.EmailService.Configurations;

public class SmtpSettings
{
    public static string SectionName => nameof(SmtpSettings);
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string SmtpHost { get; init; } = null!;
    public int Port { get; set; }
}
