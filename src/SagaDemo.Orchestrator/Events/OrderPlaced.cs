namespace SagaDemo.Orchestrator.Events;

public class OrderPlaced : Event
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }

    public const string EventType = nameof(OrderPlaced);
}
