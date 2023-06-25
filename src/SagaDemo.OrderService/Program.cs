using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SagaDemo.OrderService.Configurations;
using SagaDemo.OrderService.Data;
using SagaDemo.OrderService.Extensions;
using SagaDemo.OrderService.Persistence.Orders;
using SagaDemo.OrderService.Persistence.Products;
using SagaDemo.OrderService.Persistence.Users;
using SagaDemo.OrderService.Services.EventProcessing;
using SagaDemo.OrderService.Services.Jobs;
using SagaDemo.OrderService.Services.Orchestrator;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<BroadcastSubscriber>();
builder.Services.AddHostedService<OrchestratorSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>());
builder.Services.AddScoped<IProductsRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddAuth(builder.Configuration);


if(builder.Environment.IsDevelopment())
{
    // Database
    builder.Services.AddDbContext<OrdersDbContext>(options =>
    {
        options.UseInMemoryDatabase("OrderDatabase");
    });

    // Swagger filter
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });
}
else
{
    builder.Services.AddDbContext<OrdersDbContext>( options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
}

var app = builder.Build();
if(app.Environment.IsDevelopment())
    app.SeedData();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.PrepDatabase();

app.Run();
