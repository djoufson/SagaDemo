using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Users;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> AddUserAsync(User user);
}
