using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Users;

public class UserService : IUserService
{
    public Task<User> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
