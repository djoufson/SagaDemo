using Microsoft.EntityFrameworkCore;
using SagaDemo.AuthService.Configurations;
using SagaDemo.AuthService.Data;
using SagaDemo.AuthService.Services.Authentication;
using SagaDemo.AuthService.Services.RabbitMq;
using SagaDemo.AuthService.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AuthDbContext>(options => 
    {
        options.UseInMemoryDatabase("AuthDatabase");
    });
}
else
{
    builder.Services.AddDbContext<AuthDbContext>(options => 
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
}
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>());
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddSingleton<IBroadcastClient, BroadcastClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.SeedData();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
