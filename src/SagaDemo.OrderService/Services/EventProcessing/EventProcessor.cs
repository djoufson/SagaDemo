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
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(
        IServiceProvider serviceProvider,
        ILogger<EventProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
                    UserRegistered? content = JsonSerializer.Deserialize<UserRegistered>(message);
                    if (content is null)
                        return;

                    _logger.LogCritical("--> Create user request: {UserId}", content.Id);
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
                    ChangeOrderStateCommand? command = JsonSerializer.Deserialize<ChangeOrderStateCommand>(message);
                    if(command is null)
                        return;
                    _logger.LogCritical("--> Update order request : {OrderId}", command.OrderId);
                    _ = await orderRepository.UpdateOrderStateAsync(command.OrderId, command.State);
                    await orchestrator.RaiseOrderSuccessAsync(new OrderSuccess()
                    {
                        OrderId = command.OrderId
                    });
                }
                break;
            case UndoOrderCommand.EventType:
                {
                    UndoOrderCommand? command = JsonSerializer.Deserialize<UndoOrderCommand>(message);
                    if(command is null)
                        return;
                    _logger.LogCritical("--> Undo order request : {OrderId}", command.OrderId);
                    _ = await orderRepository.UndoOrderCommandAsync(command.OrderId);
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
