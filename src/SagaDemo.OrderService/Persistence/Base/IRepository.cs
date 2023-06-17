using SagaDemo.OrderService.Entities.Base;

namespace SagaDemo.OrderService.Persistence.Base;

public interface IRepository<T, TId>
    where T : IEntity<TId>
{
    Task<T?> GetByIdAsync(TId id);
    Task<IReadOnlyList<T>> GetAllAsync();
}
