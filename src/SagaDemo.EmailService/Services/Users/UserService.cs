using Microsoft.EntityFrameworkCore;
using SagaDemo.EmailService.Data;
using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Services.Users;

public class UserService : IUserService
{
    private readonly EmailDbContext _dbContext;

    public UserService(EmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> AddUserAsync(User user)
    {
        if(await _dbContext.Users.AnyAsync(u => u.EmailAddress == user.EmailAddress))
            return null;

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public Task<User?> GetByExternalIdAsync(Guid userId)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.ExternalId == userId);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
