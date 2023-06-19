using SagaDemo.Orchestrator.Commands;

namespace SagaDemo.Orchestrator.Services.RabbitMq;

public interface IOrchestratorClient
{
    Task NotifyOrderFailedAsync(OrderFailedCommand command);
    Task NotifyOrderPlacedAsync(OrderPlacedCommand command);
    Task RequestMakePaymentAsync(MakePaymentCommand command);
    Task RequestSendEmailAsync(SendEmailCommand command);
    Task RequestUndoOrderAsync(UndoOrderCommand command);
    Task RequestUndoPaymentAsync(UndoPaymentCommand command);
    Task RequestUpdateOrderStateAsync(ChangeOrderStateCommand command);
}
