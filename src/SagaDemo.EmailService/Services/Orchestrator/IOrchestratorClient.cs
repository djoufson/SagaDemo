using SagaDemo.EmailService.Events;

namespace SagaDemo.EmailService.Services.Orchestrator;

public interface IOrchestratorClient
{
    Task RaiseEmailSentEvent(EmailSent @event);
    Task RaiseEmailFailedEvent(EmailFailed @event);
}
