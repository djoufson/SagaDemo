using Microsoft.EntityFrameworkCore;
using SagaDemo.OrderService.Configurations;
using SagaDemo.OrderService.Data;
using SagaDemo.OrderService.Persistence.Orders;
using SagaDemo.OrderService.Persistence.Products;
using SagaDemo.OrderService.Persistence.Users;
using SagaDemo.OrderService.Services.EventProcessing;
using SagaDemo.OrderService.Services.Jobs;
using SagaDemo.OrderService.Services.Orchestrator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<OrdersDbContext>(options =>
    {
        options.UseInMemoryDatabase("OrderDatabase");
    });
}
else
{
    builder.Services.AddDbContext<OrdersDbContext>( options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
}
builder.Services.AddHostedService<BroadcastSubscriber>();
builder.Services.AddHostedService<OrchestratorSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton(_ => builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>());
builder.Services.AddScoped<IProductsRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();

var app = builder.Build();
app.SeedData();

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
