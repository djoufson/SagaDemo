using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Commands;

public class MakePaymentCommand : Event
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(MakePaymentCommand);
}
