namespace SagaDemo.EmailService.Events;
public class EmailSent : Event
{
    public Guid EmailId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(EmailSent);
}
