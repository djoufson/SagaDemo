using SagaDemo.PaymentService.Events;

namespace SagaDemo.PaymentService.Services.Orchestrator;

public interface IOrchestratorClient
{
    Task RaisePaymentSucceededEvent(PaymentSucceeded @event);
    Task RaisePaymentFailedEvent(PaymentFailed @event);
}
