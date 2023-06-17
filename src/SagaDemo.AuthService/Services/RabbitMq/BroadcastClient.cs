using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SagaDemo.AuthService.Configurations;
using SagaDemo.AuthService.Contracts;

namespace SagaDemo.AuthService.Services.RabbitMq;

public class BroadcastClient : IBroadcastClient
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<BroadcastClient> _logger;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public BroadcastClient(
        RabbitMqSettings settings, 
        ILogger<BroadcastClient> logger)
    {
        _logger = logger;
        _settings = settings;
        var factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            Port = _settings.Port
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection?.CreateModel();
            _channel?.ExchangeDeclare("broadcast", ExchangeType.Fanout);
            if(_connection is not null)
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            _logger.LogInformation("Successfully connected to RabbitMQ");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to connect to rabbitMQ");
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shut down");
    }

    public Task PublishUserRegisteredAsync(UserRegistered user)
    {
        string message = JsonSerializer.Serialize(user);
        if(_connection?.IsOpen ?? false)
        {
            _logger.LogInformation("The connection is open", message);
            SendMessage(message);
        }
        else
        {
            _logger.LogInformation("The connection is closed", message);
        }

        return Task.CompletedTask;
    }

    private void SendMessage(string message)
    {
        byte[] body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish("broadcast", "", null, body);
    }

    public void Dispose()
    {
        if(_channel?.IsOpen ?? false)
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
