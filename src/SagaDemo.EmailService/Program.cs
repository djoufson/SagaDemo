using Microsoft.EntityFrameworkCore;
using SagaDemo.EmailService.Configurations;
using SagaDemo.EmailService.Data;
using SagaDemo.EmailService.Services.EventProcessing;
using SagaDemo.EmailService.Services.Jobs;
using SagaDemo.EmailService.Services.Orchestrator;
using SagaDemo.EmailService.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddSingleton<IEventProcessor, BroadcastEventProcessor>();
builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHostedService<BroadcastSubscriber>();
builder.Services.AddHostedService<OrchestratorSubscriber>();
builder.Services.AddDbContext<EmailDbContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

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
