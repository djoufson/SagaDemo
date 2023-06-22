namespace SagaDemo.PaymentService.Entities;

public class User
{
    public Guid  Id { get; set;}
    public Guid ExternalId { get; set; }
    public decimal Balance { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
}
