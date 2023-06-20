namespace SagaDemo.OrderService.Events;

public class OrderSuccess : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    
    public const string EventType = nameof(OrderSuccess);
}
