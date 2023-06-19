using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SagaDemo.Orchestrator.Commands;
using SagaDemo.Orchestrator.Configurations;

namespace SagaDemo.Orchestrator.Services.RabbitMq;

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
            _channel?.ExchangeDeclare("orchestrator", ExchangeType.Direct);
            if(_connection is not null)
                _connection.ConnectionShutdown += (_, __) => _logger.LogWarning("RabbitMQ connection shut down");

            _logger.LogInformation("Successfully connected to RabbitMQ orchestrator queue");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to connect to rabbitMQ");
        }
    }

    public Task RequestMakePaymentAsync(MakePaymentCommand command)
    {
        SendDirectMessage(command, RoutingKeys.PaymentService);
        return Task.CompletedTask;
    }

    public Task RequestUpdateOrderStateAsync(ChangeOrderStateCommand command)
    {
        SendDirectMessage(command, RoutingKeys.OrderService);
        return Task.CompletedTask;
    }

    public Task NotifyOrderFailedAsync(OrderFailedCommand command)
    {
        SendDirectMessage(command, RoutingKeys.OrderService);
        return Task.CompletedTask;
    }

    public Task NotifyOrderPlacedAsync(OrderPlacedCommand command)
    {
        SendDirectMessage(command, RoutingKeys.OrderService);
        return Task.CompletedTask;
    }

    public Task RequestSendEmailAsync(SendEmailCommand command)
    {
        SendDirectMessage(command, RoutingKeys.EmailService);
        return Task.CompletedTask;
    }

    public Task RequestUndoOrderAsync(UndoOrderCommand command)
    {
        SendDirectMessage(command, RoutingKeys.OrderService);
        return Task.CompletedTask;
    }

    public Task RequestUndoPaymentAsync(UndoPaymentCommand command)
    {
        SendDirectMessage(command, RoutingKeys.PaymentService);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if(_channel?.IsOpen ?? false)
        {
            _channel?.Close();
            _connection?.Close();
        }
    }

    private void SendDirectMessage(object command, string routingKey = "")
    {
        string message = JsonSerializer.Serialize(command);
                if(_connection?.IsOpen ?? false)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("orchestrator", routingKey, null, body);
        }
        else
        {
            _logger.LogInformation("The connection is closed", message);
        }
    }
}

class RoutingKeys
{
    public const string EmailService = "emailService";
    public const string OrderService = "orderService";
    public const string PaymentService = "paymentService";
}
