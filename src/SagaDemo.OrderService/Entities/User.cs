using SagaDemo.OrderService.Entities.Base;

namespace SagaDemo.OrderService.Entities;

public class User : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = null!;
}
