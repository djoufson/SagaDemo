using Microsoft.EntityFrameworkCore;
using SagaDemo.PaymentService.Entities;

namespace SagaDemo.PaymentService.Data;

public class PaymentDbCOntext : DbContext
{
    public PaymentDbCOntext(DbContextOptions<PaymentDbCOntext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
}
