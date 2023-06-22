namespace SagaDemo.Orchestrator.Events;

public class PaymentFailed : Event
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public const string EventType = nameof(PaymentFailed);
}
