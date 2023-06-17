using Microsoft.AspNetCore.Mvc;
using SagaDemo.OrderService.Dtos;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Persistence.Orders;

namespace SagaDemo.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersRepository _ordersRepository;

    public OrdersController(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        return Ok(await _ordersRepository.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> MakeAnOrder(PostOrderDto orderDto)
    {
        Order? order = await _ordersRepository.MakeOrderAsync(orderDto);
        if(order is null)
            return BadRequest("Unable to make the order");

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
