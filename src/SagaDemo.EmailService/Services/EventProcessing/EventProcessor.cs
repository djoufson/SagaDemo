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
                    if (content is null)
                        return;

                    _logger.LogCritical("--> User created : {UserId}", content.Id);
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
                    if (content is null)
                        return;

                    _logger.LogCritical("--> Send email request : {UserId}", content.UserId);
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

                    // TODO: Design here to retry 3 times
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
