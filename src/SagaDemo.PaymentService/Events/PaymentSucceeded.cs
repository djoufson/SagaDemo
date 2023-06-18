namespace SagaDemo.PaymentService.Events;

public class PaymentSucceeded : Event
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(PaymentSucceeded);
}
