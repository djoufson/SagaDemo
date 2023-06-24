using Microsoft.EntityFrameworkCore;
using SagaDemo.PaymentService.Data;
using SagaDemo.PaymentService.Entities;
using SagaDemo.PaymentService.Exceptions;
using SagaDemo.PaymentService.Services.Users;

namespace SagaDemo.PaymentService.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly PaymentDbCOntext _dbContext;
    private readonly IUserRepository _userRepository;

    public PaymentService(
        PaymentDbCOntext dbContext,
        IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync()
    {
        await Task.CompletedTask;
        return await _dbContext.Transactions.ToArrayAsync();
    }

    public Task<Transaction?> GetTransactionAsync(Guid id)
    {
        return _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> MakeTransactionAsync(Transaction transaction)
    {
        User? user= await _userRepository.GetByExternalIdAsync(transaction.UserId);
        if(user is null)
            return null;
        else if(user.Balance - transaction.Amount < 0)
        {
            transaction.State = TransactionState.Fail;
        }

        // To simulate transaction process
        await Task.Delay(2000);
        await _dbContext.Transactions.AddAsync(transaction);
        if(transaction.State == TransactionState.Success)
            await _userRepository.UpdateBalanceAsync(user.Id, user.Balance - transaction.Amount);

        await _dbContext.SaveChangesAsync();
        if(transaction.State != TransactionState.Success)
            throw new NotEnoughMoneyException();

        return transaction;
    }

    public async Task<bool> UndoTransaction(Guid transactionId)
    {
        return await _dbContext.Transactions
            .Where(t => t.Id == transactionId)
            .ExecuteDeleteAsync() > 0;
    }
}
