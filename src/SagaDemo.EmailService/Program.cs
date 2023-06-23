using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using SagaDemo.EmailService.Configurations;
using SagaDemo.EmailService.Data;
using SagaDemo.EmailService.Extensions;
using SagaDemo.EmailService.Services.Emails;
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
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>());
builder.Services.AddSingleton(_ => 
{
    var smtpSettings = builder.Configuration.GetSection(SmtpSettings.SectionName).Get<SmtpSettings>();
    var smtpClient = new SmtpClient(smtpSettings.SmtpHost, smtpSettings.Port)
    {
        EnableSsl = true,
        Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password)
    };
    return smtpClient;
});
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<BroadcastSubscriber>();
builder.Services.AddHostedService<OrchestratorSubscriber>();
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<EmailDbContext>(options => 
    {
        options.UseInMemoryDatabase("EmailDatabase");
    });
}
else
{
    builder.Services.AddDbContext<EmailDbContext>(options => 
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
}

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

app.PrepDatabase();

app.Run();
