using SagaDemo.Orchestrator.Commands;

namespace SagaDemo.Orchestrator.Services.RabbitMq;

public class OrchestratorClient : IOrchestratorClient
{
    public Task NotifyMakePaymentAsync(MakePaymentCommand command)
    {
        throw new NotImplementedException();
    }

    public Task NotifyOrderFailedAsync(OrderFailedCommand command)
    {
        throw new NotImplementedException();
    }

    public Task NotifyOrderPlacedAsync(OrderPlacedCommand command)
    {
        throw new NotImplementedException();
    }

    public Task RequestSendEmailAsync(SendEmailCommand command)
    {
        throw new NotImplementedException();
    }

    public Task RequestUndoOrderAsync(UndoOrderCommand command)
    {
        throw new NotImplementedException();
    }

    public Task RequestUndoPaymentAsync(UndoPaymentCommand command)
    {
        throw new NotImplementedException();
    }
}
