using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.OrderService.Attributes;
using SagaDemo.OrderService.Dtos;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Events;
using SagaDemo.OrderService.Persistence.Orders;
using SagaDemo.OrderService.Persistence.Users;
using SagaDemo.OrderService.Services.Orchestrator;

namespace SagaDemo.OrderService.Controllers;

[ApiController]
[JwtAuthorize]
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
    [Authorize]
    public async Task<IActionResult> MakeAnOrder(PostOrderRequestDto orderDto)
    {
        string email = HttpContext.Request.Headers[HeaderKeys.EmailHeaderKey]!;
        User? user = await _userRepository.GetByEmailAsync(email);
        if(user is null)
            return Unauthorized();

        Order? order = await _ordersRepository.MakeOrderAsync(new PostOrderDto(orderDto.ProductId, user.Id, orderDto.Quantity));
        if(order is null)
            return BadRequest("Unable to make the order");

        await _orchestrator.RaiseOrderPlacedEvent(new OrderPlaced()
        {
            OrderId = order.Id,
            ProductId = order.ProductId,
            UserId = user.ExternalId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice
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
