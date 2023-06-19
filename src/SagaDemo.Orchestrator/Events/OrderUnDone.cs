namespace SagaDemo.Orchestrator.Events;

public class OrderUnDone : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(OrderUnDone);
}
