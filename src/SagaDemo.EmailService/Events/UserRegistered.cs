namespace SagaDemo.EmailService.Events;

public sealed class UserRegistered : Event
{
    public Guid Id { get; init; }
    public string Email { get; init; } = "";
    public override string EventName => EventType;

    public const string EventType = nameof(UserRegistered);
}
