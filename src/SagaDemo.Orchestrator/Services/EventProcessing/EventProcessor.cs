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
                _logger.LogCritical("--> Email successfully sent");
                break;
            case EmailFailed.EventType:
                _logger.LogCritical("--> Email failed");
                break;
            case OrderPlaced.EventType:
                {
                    OrderPlaced? orderPlaced = JsonSerializer.Deserialize<OrderPlaced>(message);
                    if(orderPlaced is null)
                        return;
                    _logger.LogCritical("--> Order Placed (Pending) : {OrderId}", orderPlaced.OrderId);
                    command = new MakePaymentCommand()
                    {
                        UserId = orderPlaced.UserId,
                        OrderId = orderPlaced.OrderId
                    };
                    await orchestrator.RequestMakePaymentAsync(command);
                }
                break;
            case OrderUnDone.EventType:
                {
                    OrderUnDone? orderCanceled = JsonSerializer.Deserialize<OrderUnDone>(message);
                    if(orderCanceled is null)
                        return;
                    _logger.LogCritical("--> Order Placed (Canceled) : {OrderId}", orderCanceled.OrderId);
                }
                break;
            case OrderSuccess.EventType:
                {
                    OrderSuccess? orderPlaced = JsonSerializer.Deserialize<OrderSuccess>(message);
                    if(orderPlaced is null)
                        return;
                    _logger.LogCritical("--> Order Placed (Success) : {OrderId}", orderPlaced.OrderId);
                }
                break;
            case PaymentFailed.EventType:
                {
                    PaymentFailed? payment = JsonSerializer.Deserialize<PaymentFailed>(message);
                    if(payment is null)
                        return;
                    _logger.LogCritical("--> Payment Failed : {PaymentId}", payment.Id);
                    command = new UndoOrderCommand()
                    {
                        OrderId = payment.OrderId
                    };
                    await orchestrator.RequestUndoOrderAsync(command);
                    command = new SendEmailCommand()
                    {
                        UserId = payment.UserId,
                        Object = "SagaDemo Notification",
                        Message = 
$@"PaymentID: {payment.Id}
OrderID: {payment.OrderId}
Payment Date: {payment.PurchaseDate.ToUniversalTime()} GMT
Status: Failed ✅"
                    };
                    await orchestrator.RequestSendEmailAsync(command);
                }
                break;
            case PaymentSucceeded.EventType:
                {
                    PaymentSucceeded? payment = JsonSerializer.Deserialize<PaymentSucceeded>(message);
                    if(payment is null)
                        return;
                    _logger.LogCritical("--> Payment Succeeded : {PaymentId}", payment.Id);
                    command = new SendEmailCommand()
                    {
                        UserId = payment.UserId,
                        Object = "SagaDemo Notification",
                        Message = 
$@"PaymentID: {payment.Id}
OrderID: {payment.OrderId}
PaymentDate: {payment.PurchaseDate.ToUniversalTime()} GMT
Status: Success ✅"
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
