namespace SagaDemo.OrderService.EventProcessing;

public interface IEventProcessor
{
    void Process(string message);
}
