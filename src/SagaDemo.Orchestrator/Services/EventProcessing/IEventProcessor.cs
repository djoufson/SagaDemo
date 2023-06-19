namespace SagaDemo.Orchestrator.Services.EventProcessing;

public interface IEventProcessor
{
    void Process(string message);
}
