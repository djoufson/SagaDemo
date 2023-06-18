using SagaDemo.EmailService.Events;

namespace SagaDemo.EmailService.Commands;

public sealed class SendEmailCommand : Event
{
    public string ToUserEmail { get; init; } = null!;
    public string Object { get; init; } = null!;
    public string Message { get; init; } = null!;
    public override string EventName => EventType;
    public const string EventType = nameof(SendEmailCommand);
}
