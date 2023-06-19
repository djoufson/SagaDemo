using System.Text.Json;
using SagaDemo.EmailService.Commands;
using SagaDemo.EmailService.Entities;
using SagaDemo.EmailService.Events;
using SagaDemo.EmailService.Services.Emails;
using SagaDemo.EmailService.Services.Orchestrator;
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
        IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        IOrchestratorClient orchestrator = scope.ServiceProvider.GetRequiredService<IOrchestratorClient>();
        Event? @event = JsonSerializer.Deserialize<Event>(message);
        if (@event is null)
            return;

        switch (@event.EventName)
        {
            case UserRegistered.EventType:
                {
                    UserRegistered? content = JsonSerializer.Deserialize<UserRegistered>(message);
                    Console.WriteLine($"--> User created");
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
            case SendEmailCommand.EventType:
                {
                    SendEmailCommand? content = JsonSerializer.Deserialize<SendEmailCommand>(message);
                    Console.WriteLine($"--> Send email request");
                    if (content is null)
                        return;
                    User? user = await userService.GetByIdAsync(content.UserId);
                    if(user is null)
                        return;

                    Email email = new ()
                    {
                        ToUserEmail = user.EmailAddress,
                        Object = content.Object,
                        Message = content.Message,
                        State = EmailState.Pending
                    };
                    bool success = await emailService.SendAsync(email);
                    if(success)
                    {
                        EmailSent sent = new()
                        {
                            EmailId = email.Id
                        };
                        await orchestrator.RaiseEmailSentEvent(sent);
                    }
                    else
                    {
                        EmailFailed failed = new()
                        {
                            EmailId = email.Id
                        };
                        await orchestrator.RaiseEmailFailedEvent(failed);
                    }
                }
                break;
            default:
                break;
        }
    }
}
