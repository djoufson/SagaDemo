using Microsoft.EntityFrameworkCore;
using SagaDemo.PaymentService.Data;
using SagaDemo.PaymentService.Entities;

namespace SagaDemo.PaymentService.Services.Users;

internal class UserRepository : IUserRepository
{
    private readonly PaymentDbCOntext _dbContext;

    public UserRepository(PaymentDbCOntext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> AddUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public Task<User?> GetByExternalIdAsync(Guid id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.ExternalId == id);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> UpdateBalanceAsync(Guid id, decimal amount)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if(user is null)
            return false;
        user.Balance = amount;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
