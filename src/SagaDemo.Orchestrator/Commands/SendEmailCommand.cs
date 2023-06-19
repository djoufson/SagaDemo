using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Commands;

public sealed class SendEmailCommand : Event
{
    public Guid UserId { get; init; }
    public string Object { get; init; } = null!;
    public string Message { get; init; } = null!;
    public override string EventName => EventType;
    public const string EventType = nameof(SendEmailCommand);
}
