using Microsoft.AspNetCore.Mvc;
using SagaDemo.OrderService.Dtos;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Events;
using SagaDemo.OrderService.Persistence.Orders;
using SagaDemo.OrderService.Persistence.Users;
using SagaDemo.OrderService.Services.Orchestrator;

namespace SagaDemo.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrchestratorClient _orchestrator;

    public OrdersController(
        IOrdersRepository ordersRepository,
        IOrchestratorClient orchestrator,
        IUserRepository userRepository)
    {
        _ordersRepository = ordersRepository;
        _orchestrator = orchestrator;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        return Ok(await _ordersRepository.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> MakeAnOrder(PostOrderDto orderDto)
    {
        User? user = await _userRepository.GetByIdAsync(orderDto.UserId);
        Order? order = await _ordersRepository.MakeOrderAsync(orderDto);
        if(order is null || user is null)
            return BadRequest("Unable to make the order");

        await _orchestrator.RaiseOrderPlacedEvent(new OrderPlaced()
        {
            OrderId = order.Id,
            ProductId = order.ProductId,
            UserId = user.ExternalId,
            Quantity = order.Quantity
        });
        return Ok(order);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        bool success = await _ordersRepository.DeleteOrderAsync(id);
        if(!success)
            return BadRequest("An error occurred while attempting to delete the order");
        return Ok();
    }
}
