using System.Text.Json;
using SagaDemo.Orchestrator.Events;

namespace SagaDemo.Orchestrator.Services.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public EventProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async void Process(string message)
    {
        await Task.CompletedTask;
        using var scope = _serviceProvider.CreateScope();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            default:
                break;
        }
    }
}
