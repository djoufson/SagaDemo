using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Persistence.Base;

namespace SagaDemo.OrderService.Persistence.Users;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> AddUserAsync(User user);
}
