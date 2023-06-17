using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Persistence.Base;

namespace SagaDemo.OrderService.Persistence.Products;

public interface IProductsRepository : IRepository<Product, Guid>
{
}
