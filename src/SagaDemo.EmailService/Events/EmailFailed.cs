namespace SagaDemo.EmailService.Events;

public class EmailFailed : Event
{
    public Guid EmailId { get; set; }
    public new string EventName => EventType;
    public const string EventType = nameof(EmailFailed);
}
