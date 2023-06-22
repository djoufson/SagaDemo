using SagaDemo.AuthService.Models;

namespace SagaDemo.AuthService.Services.Users;

public interface IUserService
{
    Task<User?> RegisterUserAsync(User user);
    Task<string?> LoginUserAsync(string email, string password);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<bool> ExistsAsync(string email);
}
