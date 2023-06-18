using System.Text.Json;
using SagaDemo.EmailService.Entities;
using SagaDemo.EmailService.Events;
using SagaDemo.EmailService.Services.Users;

namespace SagaDemo.EmailService.Services.EventProcessing;

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
        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
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
                        ExternalId = content.Id,
                        EmailAddress = content.Email,
                    };
                    await userService.AddUserAsync(user);
                }
                break;
            default:
                break;
        }
    }
}
