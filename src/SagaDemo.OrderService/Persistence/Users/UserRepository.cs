using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Data;
using SagaDemo.OrderService.Entities;

namespace SagaDemo.OrderService.Persistence.Users;

public class UserRepository : IUserRepository
{
    private readonly OrdersDbContext _dbContext;

    public UserRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> AddUserAsync(User user)
    {
        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await _dbContext.Users.ToArrayAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
