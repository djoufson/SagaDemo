using SagaDemo.EmailService.Services.Jobs;
using SagaDemo.Orchestrator.Configurations;
using SagaDemo.Orchestrator.Services.EventProcessing;
using SagaDemo.Orchestrator.Services.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>());
builder.Services.AddHostedService<EmailServiceSubscriber>();
builder.Services.AddHostedService<OrderServiceSubscriber>();
builder.Services.AddHostedService<PaymentServiceSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
