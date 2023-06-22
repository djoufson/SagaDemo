namespace SagaDemo.EmailService.Configurations;

public class EmailSettings
{
    public static string SectionName => nameof(EmailSettings);
    public string FromEmail { get; init; } = null!;
}
