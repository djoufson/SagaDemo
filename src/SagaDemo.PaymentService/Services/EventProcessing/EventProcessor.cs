using System.Text.Json;
using SagaDemo.PaymentService.Commands;
using SagaDemo.PaymentService.Entities;
using SagaDemo.PaymentService.Events;
using SagaDemo.PaymentService.Exceptions;
using SagaDemo.PaymentService.Services.Orchestrator;
using SagaDemo.PaymentService.Services.Payments;
using SagaDemo.PaymentService.Services.Users;

namespace SagaDemo.PaymentService.Services.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(
        IServiceProvider serviceProvider,
        ILogger<EventProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async void Process(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        IPaymentService paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        IOrchestratorClient orchestratorClient = scope.ServiceProvider.GetRequiredService<IOrchestratorClient>();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            case UserRegistered.EventType:
                {
                    UserRegistered? content = JsonSerializer.Deserialize<UserRegistered>(message);
                    if (content is null)
                        return;

                    _logger.LogCritical("--> Make payment request : {UserId}", content.Id);
                    var user = new User()
                    {
                        ExternalId = content.Id,
                        Balance = 2000,
                    };
                    await userRepository.AddUserAsync(user);
                }
                break;
            case MakePaymentCommand.EventType:
                {
                    MakePaymentCommand? content = JsonSerializer.Deserialize<MakePaymentCommand>(message);
                    if (content is null)
                        return;

                    _logger.LogCritical("--> Make payment request : {OrderId}", content.OrderId);
                    Transaction? transaction = new()
                    {
                        UserId = content.UserId,
                        OrderId = content.OrderId,
                        Amount = content.Amount,
                        PurchaseDate = DateTime.Now,
                        State = TransactionState.Success,
                    };
                    try
                    {
                        await paymentService.MakeTransactionAsync(transaction);
                        if (transaction is null || transaction.State != TransactionState.Success)
                        {
                            await orchestratorClient.RaisePaymentFailedEvent(new PaymentFailed()
                            {
                                Id = transaction?.Id ?? Guid.Empty,
                                UserId = transaction?.UserId ?? Guid.Empty,
                                OrderId = transaction?.OrderId ?? Guid.Empty,
                                PurchaseDate = transaction?.PurchaseDate ?? DateTime.Now,
                                Reason = "The authenticated user does not exist"
                            });
                        }
                        else
                        {
                            await orchestratorClient.RaisePaymentSucceededEvent(new PaymentSucceeded()
                            {
                                Id = transaction.Id,
                                UserId = transaction.UserId,
                                OrderId = transaction.OrderId,
                                PurchaseDate = transaction.PurchaseDate
                            });
                        }
                    }
                    catch (NotEnoughMoneyException e)
                    {
                        _logger.LogError("The payment couldn't be proceed: {Reason}", e.Message);
                        await orchestratorClient.RaisePaymentFailedEvent(new PaymentFailed()
                        {
                            Id = transaction?.Id ?? Guid.Empty,
                            UserId = transaction?.UserId ?? Guid.Empty,
                            OrderId = transaction?.OrderId ?? Guid.Empty,
                            PurchaseDate = transaction?.PurchaseDate ?? DateTime.Now,
                            Reason = e.Message
                        });
                    }
                    break;
                }
            case UndoMakePaymentCommand.EventType:
                {
                    UndoMakePaymentCommand? content = JsonSerializer.Deserialize<UndoMakePaymentCommand>(message);
                    if (content is null)
                        return;

                    _logger.LogCritical("--> Undo payment request : {TransactionId}", content.TransactionId);
                    await paymentService.UndoTransaction(content.TransactionId);
                }
                break;
            default:
                break;
        }
    }
}
