using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Commands;

public class UndoPaymentCommand : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(UndoPaymentCommand);
}
