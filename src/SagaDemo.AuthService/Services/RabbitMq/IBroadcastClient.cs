using SagaDemo.AuthService.Contracts;

namespace SagaDemo.AuthService.Services.RabbitMq;

public interface IBroadcastClient
{
    Task PublishUserRegisteredAsync(UserRegistered @event);
}
