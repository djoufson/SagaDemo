using SagaDemo.OrderService.Dtos;
using SagaDemo.OrderService.Entities;

namespace SagaDemo.OrderService.Extensions;

internal static class OrderExtensions
{
    public static Order CreateOrder(this PostOrderDto orderDto, Product product)
    {
        return new Order()
        {
            // Id = orderId,
            ProductId = product.Id,
            Product = product,
            UserId = orderDto.UserId,
            Quantity = orderDto.Quantity,
            State = OrderState.Pending
        };
    }

    // public static Order CreateOrder(this PostOrderDto orderDto, Product product)
    // {
    //     return CreateOrder(orderDto, Guid.NewGuid(), product);
    // }
}
