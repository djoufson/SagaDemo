using SagaDemo.EmailService.Events;

namespace SagaDemo.EmailService.Commands;

public sealed class SendEmailCommand : Event
{
    public Guid UserId { get; set; }
    public string Object { get; init; } = null!;
    public string Message { get; init; } = null!;
    public const string EventType = nameof(SendEmailCommand);
}
