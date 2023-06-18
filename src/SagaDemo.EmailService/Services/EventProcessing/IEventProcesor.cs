namespace SagaDemo.EmailService.Services.EventProcessing;

public interface IEventProcessor
{
    void Process(string message);
}
