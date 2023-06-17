using SagaDemo.AuthService.Models;

namespace SagaDemo.AuthService.Services.Users;

public interface IUserService
{
    Task<User?> RegisterUserAsync(User user);
    Task<string?> LoginUserAsync(string email, string password);
    // Task<User?> GetByIdAsync(Guid id);
    // Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
}
