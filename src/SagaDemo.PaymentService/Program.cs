using Microsoft.EntityFrameworkCore;
using SagaDemo.PaymentService.Configurations;
using SagaDemo.PaymentService.Data;
using SagaDemo.PaymentService.Services.EventProcessing;
using SagaDemo.PaymentService.Services.Jobs;
using SagaDemo.PaymentService.Services.Orchestrator;
using SagaDemo.PaymentService.Services.Payments;
using SagaDemo.PaymentService.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if(builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<PaymentDbCOntext>(options =>
    {
        options.UseInMemoryDatabase("PaymentsDatabase");
    });
}
else
{
    builder.Services.AddDbContext<PaymentDbCOntext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
}
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHostedService<OrchestratorSubscriber>();
builder.Services.AddHostedService<BroadcastSubscriber>();
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
