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
                Console.WriteLine($"--> Parsing the event content: {content}");
                if (content is null)
                    return;

                Transaction transaction = new()
                {
                    OrderId = content.OrderId,
                    PurchaseDate = DateTime.Now,
                    State = TransactionState.Pending,
                };
                transaction = await paymentService.MakeTransactionAsync(transaction);
                if(transaction.State == TransactionState.Success)
                {
                    await orchestratorClient.RaisePaymentSucceededEvent(new PaymentSucceeded()
                    {
                        Id = transaction.Id,
                        OrderId = transaction.OrderId,
                        PurchaseDate = transaction.PurchaseDate
                    });
                }
                else
                {
                    await orchestratorClient.RaisePaymentFailedEvent(new PaymentFailed()
                    {
                        Id = transaction.Id,
                        OrderId = transaction.OrderId,
                        PurchaseDate = transaction.PurchaseDate
                    });
                }
                break;
            }
            default:
                break;
        }
    }
}
