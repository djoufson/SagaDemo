using System.Text.Json;
using SagaDemo.Orchestrator.Commands;
using SagaDemo.Orchestrator.Events;
using SagaDemo.Orchestrator.Services.RabbitMq;

namespace SagaDemo.Orchestrator.Services.EventProcessing;

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
        dynamic command;
        using var scope = _serviceProvider.CreateScope();
        IOrchestratorClient orchestrator = scope.ServiceProvider.GetRequiredService<IOrchestratorClient>();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            case EmailSent.EventType:
                Console.WriteLine("--> Email Sent");
                break;
            case EmailFailed.EventType:
                Console.WriteLine("--> Email Failed");
                break;
            case OrderPlaced.EventType:
                {
                    _logger.LogCritical("--> Order Placed (Pending)");
                    OrderPlaced? orderPlaced = JsonSerializer.Deserialize<OrderPlaced>(message);
                    if(orderPlaced is null)
                        return;
                    command = new MakePaymentCommand()
                    {
                        OrderId = orderPlaced.OrderId
                    };
                    await orchestrator.RequestMakePaymentAsync(command);
                }
                break;
            case PaymentFailed.EventType:
                {
                    _logger.LogCritical("--> Payment Failed");
                    PaymentFailed? payment = JsonSerializer.Deserialize<PaymentFailed>(message);
                    if(payment is null)
                        return;
                    command = new UndoOrderCommand()
                    {
                        OrderId = payment.OrderId
                    };
                    await orchestrator.RequestUndoOrderAsync(command);
                }
                break;
            case PaymentSucceeded.EventType:
                {
                    _logger.LogCritical("--> Payment Succeeded");
                    PaymentSucceeded? payment = JsonSerializer.Deserialize<PaymentSucceeded>(message);
                    if(payment is null)
                        return;
                    command = new SendEmailCommand()
                    {
                        UserId = payment.UserId,
                        Object = "SagaDemo Notification",
                        Message = $@"
                        PaymentID: {payment.Id}
                        OrderID: {payment.OrderId}
                        PaymentDate: {payment.PurchaseDate}
                        Status: Success"
                    };
                    await orchestrator.RequestSendEmailAsync(command);
                    command = new ChangeOrderStateCommand()
                    {
                        OrderId = payment.OrderId,
                        State = OrderState.Success
                    };
                    await orchestrator.RequestUpdateOrderStateAsync(command);
                }
                break;
            default:
                break;
        }
    }
}
