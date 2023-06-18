using System.Text.Json;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Events;
using SagaDemo.OrderService.Persistence.Users;

namespace SagaDemo.OrderService.EventProcessing;

public class BroadcastEventProcessor : IEventProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public BroadcastEventProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async void Process(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            case UserRegistered.EventType:
                {
                    UserRegistered? content = JsonSerializer.Deserialize<UserRegistered>(message);
                    Console.WriteLine($"--> Parsing the event content: {content}");
                    if (content is null)
                        return;

                    User user = new()
                    {
                        Name = content.Name,
                        ExternalId = content.Id,
                        Email = content.Email,
                        Orders = Array.Empty<Order>(),
                    };
                    await userRepository.AddUserAsync(user);
                }
                break;
            default:
                break;
        }
    }
}
