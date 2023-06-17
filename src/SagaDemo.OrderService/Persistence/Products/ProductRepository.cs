using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Data;
using SagaDemo.OrderService.Entities;

namespace SagaDemo.OrderService.Persistence.Products;

public class ProductRepository : IProductsRepository
{
    private readonly OrdersDbContext _dbContext;

    public ProductRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        return await _dbContext.Products.ToArrayAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }
}
