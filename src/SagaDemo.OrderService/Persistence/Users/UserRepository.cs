using SagaDemo.OrderService.Entities;

namespace SagaDemo.OrderService.Persistence.Users;

public class UserRepository : IUserRepository
{
    public Task<IReadOnlyList<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
