namespace SagaDemo.PaymentService.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public TransactionState State { get; set; }
}

public enum TransactionState
{
    Pending,
    Success,
    Cancelled,
    Fail,
}
