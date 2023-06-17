using SagaDemo.OrderService.Entities.Base;

namespace SagaDemo.OrderService.Entities;

public class Product : IEntity<Guid>
{
    public Guid Id { get ; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
}
