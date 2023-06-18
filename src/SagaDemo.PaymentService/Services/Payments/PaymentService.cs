using Microsoft.EntityFrameworkCore;
using SagaDemo.PaymentService.Data;
using SagaDemo.PaymentService.Entities;

namespace SagaDemo.PaymentService.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly PaymentDbCOntext _dbContext;

    public PaymentService(PaymentDbCOntext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Transaction?> GetTransactionAsync(Guid id)
    {
        return _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> MakeTransactionAsync(Transaction transaction)
    {
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }
}
