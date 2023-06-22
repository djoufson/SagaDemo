namespace SagaDemo.PaymentService.Events;

public class UserRegistered
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string EventName => EventType;
    public const string EventType = nameof(UserRegistered);
}
