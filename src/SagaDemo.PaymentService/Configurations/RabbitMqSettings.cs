namespace SagaDemo.PaymentService.Configurations;

public class RabbitMqSettings
{
    public static string SectionName => nameof(RabbitMqSettings);
    public string Host { get; init; } = null!;
    public int Port { get; init; }
}
