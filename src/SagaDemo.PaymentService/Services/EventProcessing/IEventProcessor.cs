namespace SagaDemo.PaymentService.Services.EventProcessing;

public interface IEventProcessor
{
    void Process(string message);
}
