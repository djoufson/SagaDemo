namespace SagaDemo.PaymentService.Events;

public class PaymentFailed : Event
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Reason { get; set; } = null!;
    public override string EventName => EventType;
    public const string EventType = nameof(PaymentFailed);
}
