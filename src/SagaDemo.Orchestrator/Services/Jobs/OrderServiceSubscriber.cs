using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaDemo.Orchestrator.Configurations;
using SagaDemo.Orchestrator.Services.EventProcessing;

namespace SagaDemo.EmailService.Services.Jobs;

public class OrderServiceSubscriber : BackgroundService
{
    private readonly IEventProcessor _eventProcessor;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _queueName;
    private readonly ILogger<OrderServiceSubscriber> _logger;

    public OrderServiceSubscriber(
        IEventProcessor eventProcessor,
        RabbitMqSettings rabbitMqSettings,
        ILogger<OrderServiceSubscriber> logger)
    {
        _eventProcessor = eventProcessor;
        _settings = rabbitMqSettings;
        _logger = logger;
        InitializeRabbitMq();
    }

    public void InitializeRabbitMq()
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

        _channel?.QueueBind(_queueName, "orchestrator", "orderService");
        if (_connection is not null)
            _connection.ConnectionShutdown += (_, __) => _logger.LogWarning("RabbitMQ connection shut down");

        _logger.LogInformation("Successfully connected to RabbitMQ order service queue");
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

    public new void Dispose()
    {
        if (_channel?.IsOpen ?? false)
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
