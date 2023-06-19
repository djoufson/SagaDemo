using System.Text.Json;
using SagaDemo.PaymentService.Commands;
using SagaDemo.PaymentService.Entities;
using SagaDemo.PaymentService.Events;
using SagaDemo.PaymentService.Services.Orchestrator;
using SagaDemo.PaymentService.Services.Payments;

namespace SagaDemo.PaymentService.Services.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public EventProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async void Process(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        IPaymentService paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        IOrchestratorClient orchestratorClient = scope.ServiceProvider.GetRequiredService<IOrchestratorClient>();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            case MakePaymentCommand.EventType:
                {
                    MakePaymentCommand? content = JsonSerializer.Deserialize<MakePaymentCommand>(message);
                    Console.WriteLine($"--> Make payment request");
                    if (content is null)
                        return;

                    Transaction transaction = new()
                    {
                        UserId = content.UserId,
                        OrderId = content.OrderId,
                        PurchaseDate = DateTime.Now,
                        State = TransactionState.Success,
                    };
                    transaction = await paymentService.MakeTransactionAsync(transaction);
                    if(transaction.State == TransactionState.Success)
                    {
                        await orchestratorClient.RaisePaymentSucceededEvent(new PaymentSucceeded()
                        {
                            Id = transaction.Id,
                            UserId = transaction.UserId,
                            OrderId = transaction.OrderId,
                            PurchaseDate = transaction.PurchaseDate
                        });
                    }
                    else
                    {
                        await orchestratorClient.RaisePaymentFailedEvent(new PaymentFailed()
                        {
                            Id = transaction.Id,
                            UserId = transaction.UserId,
                            OrderId = transaction.OrderId,
                            PurchaseDate = transaction.PurchaseDate
                        });
                    }
                    break;
                }
            case UndoMakePaymentCommand.EventType:
                {
                    UndoMakePaymentCommand? content = JsonSerializer.Deserialize<UndoMakePaymentCommand>(message);
                    Console.WriteLine($"--> Undo payment request");
                    if (content is null)
                        return;

                    await paymentService.UndoTransaction(content.TransactionId);
                }
                break;
            default:
                break;
        }
    }
}
