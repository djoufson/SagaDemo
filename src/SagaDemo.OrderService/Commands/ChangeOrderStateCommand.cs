using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Events;

namespace SagaDemo.OrderService.Commands;

public class ChangeOrderStateCommand : Event
{
    public Guid OrderId { get; set; }
    public OrderState State { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(ChangeOrderStateCommand);
}
