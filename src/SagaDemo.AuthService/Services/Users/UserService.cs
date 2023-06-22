using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SagaDemo.AuthService.Data;
using SagaDemo.AuthService.Models;
using SagaDemo.AuthService.Services.Authentication;

namespace SagaDemo.AuthService.Services.Users;

internal class UserService : IUserService
{
    private readonly AuthDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public UserService(
        AuthDbContext dbContext,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public Task<bool> ExistsAsync(string email)
    {
        return _dbContext.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var users = await _dbContext.Users.ToArrayAsync();
        return users.ToArray();
    }

    public async Task<string?> LoginUserAsync(string email, string password)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if(user is null)
            return null;

        if(user.Password != password)
            return null;

        Claim[] claims = new []
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, user.Name)
        };
        
        return _jwtTokenGenerator.GenerateToken(claims);
    }

    public async Task<User?> RegisterUserAsync(User user)
    {
        bool exists = await ExistsAsync(user.Email);
        if(exists)
            return null;

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}
