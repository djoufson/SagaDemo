using Microsoft.EntityFrameworkCore;
using SagaDemo.EmailService.Entities;

namespace SagaDemo.EmailService.Data;

public class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}
