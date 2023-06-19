using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Commands;

public class UndoOrderCommand : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(UndoOrderCommand);
}
