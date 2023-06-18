namespace SagaDemo.OrderService.Services.EventProcessing;

public interface IEventProcessor
{
    void Process(string message);
}
