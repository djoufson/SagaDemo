using SagaDemo.PaymentService.Entities;

namespace SagaDemo.PaymentService.Services.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByExternalIdAsync(Guid id);
    Task<User?> AddUserAsync(User user);
    Task<bool> UpdateBalanceAsync(Guid id, decimal amount);
}
