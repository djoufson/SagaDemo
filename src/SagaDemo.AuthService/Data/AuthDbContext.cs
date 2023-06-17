using Microsoft.EntityFrameworkCore;
using SagaDemo.AuthService.Models;

namespace SagaDemo.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}
