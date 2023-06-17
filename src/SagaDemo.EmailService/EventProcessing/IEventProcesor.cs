namespace SagaDemo.EmailService.EventProcessing;

public interface IEventProcessor
{
    void Process(string message);
}
