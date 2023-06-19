using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Commands;

public class OrderFailedCommand : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(OrderFailedCommand);
}
