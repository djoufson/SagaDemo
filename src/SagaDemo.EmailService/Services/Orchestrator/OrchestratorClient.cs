using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SagaDemo.EmailService.Configurations;
using SagaDemo.EmailService.Events;

namespace SagaDemo.EmailService.Services.Orchestrator;

public class OrchestratorClient : IOrchestratorClient
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<OrchestratorClient> _logger;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public OrchestratorClient(
        RabbitMqSettings settings,
        ILogger<OrchestratorClient> logger)
    {
        _settings = settings;
        _logger = logger;
        var factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            Port = _settings.Port
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection?.CreateModel();
            _channel?.ExchangeDeclare("orchestrator", ExchangeType.Direct);
            if(_connection is not null)
                _connection.ConnectionShutdown += (_, __) => _logger.LogCritical("RabbitMQ connection shut down");

            _logger.LogInformation("Successfully connected to RabbitMQ direct exchange");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to connect to rabbitMQ");
        }
    }

    public Task RaiseEmailSentEvent(EmailSent @event)
    {
        string message = JsonSerializer.Serialize(@event);
        RaiseEvent(message);
        return Task.CompletedTask;
    }

    public Task RaiseEmailFailedEvent(EmailFailed @event)
    {
        string message = JsonSerializer.Serialize(@event);
        RaiseEvent(message);
        return Task.CompletedTask;
    }

    private void RaiseEvent(string message)
    {
        if(_connection?.IsOpen ?? false)
        {
            _logger.LogInformation("The connection is open", message);
            SendMessage(message);
        }
        else
            _logger.LogInformation("The connection is closed", message);
    }

    #region Setup methods
    private void SendMessage(string message)
    {
        byte[] body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish("orchestrator", "emailService", null, body);
    }

    public void Dispose()
    {
        if(_channel?.IsOpen ?? false)
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
    #endregion
}
