using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaDemo.EmailService.Configurations;
using SagaDemo.EmailService.Services.EventProcessing;

namespace SagaDemo.EmailService.Services.Jobs;

public class OrchestratorSubscriber : BackgroundService
{    
    private readonly IEventProcessor _eventProcessor;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _queueName;
    private readonly ILogger<BroadcastSubscriber> _logger;

    public OrchestratorSubscriber(
        IEventProcessor eventProcessor,
        RabbitMqSettings rabbitMqSettings,
        ILogger<BroadcastSubscriber> logger)
    {
        _eventProcessor = eventProcessor;
        _settings = rabbitMqSettings;
        _logger = logger;
        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            Port = _settings.Port
        };
        _connection = factory.CreateConnection();
        _channel = _connection?.CreateModel();
        _channel?.ExchangeDeclare("orchestrator", ExchangeType.Direct);
        _queueName = _channel?.QueueDeclare().QueueName;

        _channel?.QueueBind(_queueName, "orchestrator", "emailService");
        if (_connection is not null)
            _connection.ConnectionShutdown += (_, __) => _logger.LogWarning("RabbitMQ connection shut down");

        _logger.LogInformation("Successfully connected to RabbitMQ");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += EventReceived;
        _channel.BasicConsume(_queueName, true, consumer);
        return Task.CompletedTask;
    }

    private void EventReceived(object? sender, BasicDeliverEventArgs e)
    {
        string message = Encoding.UTF8.GetString(e.Body.ToArray());
        _eventProcessor.Process(message);
    }
}
