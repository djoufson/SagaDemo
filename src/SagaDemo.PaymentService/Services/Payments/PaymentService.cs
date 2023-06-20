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
        bool success = Random.Shared.Next(1, 20) <= 8;
        if(success)
            transaction.State = TransactionState.Success;
        else
            transaction.State = TransactionState.Fail;
        await Task.Delay(2000);
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }

    public async Task<bool> UndoTransaction(Guid transactionId)
    {
        return await _dbContext.Transactions
            .Where(t => t.Id == transactionId)
            .ExecuteDeleteAsync() > 0;
    }
}
