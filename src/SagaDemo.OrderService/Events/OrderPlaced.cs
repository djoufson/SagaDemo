namespace SagaDemo.OrderService.Events;

public class OrderPlaced : Event
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public override string EventName => EventType;
    
    public const string EventType = nameof(OrderPlaced);
}
