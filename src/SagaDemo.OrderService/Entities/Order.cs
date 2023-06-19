using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SagaDemo.OrderService.Entities.Base;

namespace SagaDemo.OrderService.Entities;

public class Order : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public OrderState State { get; set; }
}

public enum OrderState
{
    Pending,
    Success,
    Fail
}
