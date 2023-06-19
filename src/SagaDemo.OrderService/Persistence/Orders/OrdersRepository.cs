using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Data;
using SagaDemo.OrderService.Dtos;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Persistence.Products;

namespace SagaDemo.OrderService.Persistence.Orders;

public class OrdersRepository : IOrdersRepository
{
    private readonly OrdersDbContext _dbContext;
    private readonly IProductsRepository _productsRepository;

    public OrdersRepository(
        OrdersDbContext dbContext,
        IProductsRepository productsRepository)
    {
        _dbContext = dbContext;
        _productsRepository = productsRepository;
    }

    public async Task<bool> DeleteOrderAsync(Guid orderId)
    {
        int records = await _dbContext.Orders
            .Where(o => o.Id == orderId)
            .ExecuteDeleteAsync();
        return records > 0;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync()
    {
        return await _dbContext.Orders
            // .Include(o => o.Product)
            .ToArrayAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Orders
            // .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> MakeOrderAsync(PostOrderDto orderDto)
    {
        Product? product = await _productsRepository.GetByIdAsync(orderDto.ProductId);
        if(product is null)
            return null;

        Order order = new()
        {
            ProductId = orderDto.ProductId,
            UserId = orderDto.UserId,
            Product = product,
            State = OrderState.Pending
        };
        await _dbContext.Orders.AddRangeAsync(order);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateOrderStateAsync(Guid orderId, OrderState state)
    {
        Order? order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if(order is null)
            return null;
        order.State = state;
        await _dbContext.SaveChangesAsync();
        return order;
    }
}
