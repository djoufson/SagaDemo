namespace SagaDemo.OrderService.Events;

public record UserRegistered(
    Guid Id,
    string Name,
    string Email,
    string EventName
)
{
    public const string EventType = nameof(UserRegistered);
}
