using Microsoft.EntityFrameworkCore;
using SagaDemo.PaymentService.Configurations;
using SagaDemo.PaymentService.Data;
using SagaDemo.PaymentService.Services.EventProcessing;
using SagaDemo.PaymentService.Services.Jobs;
using SagaDemo.PaymentService.Services.Orchestrator;
using SagaDemo.PaymentService.Services.Payments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PaymentDbCOntext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddHostedService<OrchestratorSubscriber>();
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
