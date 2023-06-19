using System.Text.Json;
using SagaDemo.OrderService.Commands;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Events;
using SagaDemo.OrderService.Persistence.Orders;
using SagaDemo.OrderService.Persistence.Users;
using SagaDemo.OrderService.Services.Orchestrator;

namespace SagaDemo.OrderService.Services.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public EventProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async void Process(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        IOrdersRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        IOrchestratorClient orchestrator = scope.ServiceProvider.GetRequiredService<IOrchestratorClient>();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            case UserRegistered.EventType:
                {
                    Console.WriteLine("--> User create request");
                    UserRegistered? content = JsonSerializer.Deserialize<UserRegistered>(message);
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
            case ChangeOrderStateCommand.EventType:
                {
                    Console.WriteLine("--> Order update request");
                    ChangeOrderStateCommand? command = JsonSerializer.Deserialize<ChangeOrderStateCommand>(message);
                    if(command is null)
                        return;
                    _ = await orderRepository.UpdateOrderStateAsync(command.OrderId, command.State);
                    await orchestrator.RaiserOrderUnDoneAsync(new OrderUnDone()
                    {
                        OrderId = command.OrderId
                    });
                }
                break;
            default:
                break;
        }
    }
}
