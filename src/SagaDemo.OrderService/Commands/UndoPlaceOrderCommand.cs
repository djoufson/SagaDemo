using SagaDemo.OrderService.Events;

namespace SagaDemo.OrderService.Commands;

public class UndoPlaceOrderCommand : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(UndoPlaceOrderCommand);
}
