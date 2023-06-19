using SagaDemo.OrderService.Dtos;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Persistence.Base;

namespace SagaDemo.OrderService.Persistence.Orders;

public interface IOrdersRepository : IRepository<Order, Guid>
{
    Task<Order?> MakeOrderAsync(PostOrderDto order);
    Task<bool> DeleteOrderAsync(Guid orderId);
    Task<Order?> UpdateOrderStateAsync(Guid orderId, OrderState state);
}
