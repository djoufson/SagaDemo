namespace SagaDemo.Orchestrator.Events;

public class EmailFailed : Event
{
    public Guid EmailId { get; set; }
    public const string EventType = nameof(EmailFailed);
}
