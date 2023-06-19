namespace SagaDemo.Orchestrator.Events;
public class EmailSent : Event
{
    public Guid EmailId { get; set; }
    public const string EventType = nameof(EmailSent);
}
