using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Commands;

public class ChangeOrderStateCommand : Event
{
    public Guid OrderId { get; set; }
    public OrderState State { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(ChangeOrderStateCommand);
}

public enum OrderState
{
    Pending,
    Success,
    Fail
}
