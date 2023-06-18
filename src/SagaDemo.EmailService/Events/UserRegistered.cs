namespace SagaDemo.EmailService.Events;

public record UserRegistered(
    Guid Id,
    string Email,
    string EventName
)
{
    public const string EventType = nameof(UserRegistered);
}
