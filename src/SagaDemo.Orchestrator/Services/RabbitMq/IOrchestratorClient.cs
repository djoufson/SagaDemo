using SagaDemo.Orchestrator.Commands;

namespace SagaDemo.Orchestrator.Services.RabbitMq;

public interface IOrchestratorClient
{
    Task NotifyMakePaymentAsync(MakePaymentCommand command);
    Task NotifyOrderFailedAsync(OrderFailedCommand command);
    Task NotifyOrderPlacedAsync(OrderPlacedCommand command);
    Task RequestSendEmailAsync(SendEmailCommand command);
    Task RequestUndoOrderAsync(UndoOrderCommand command);
    Task RequestUndoPaymentAsync(UndoPaymentCommand command);
}
