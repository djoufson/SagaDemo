using Microsoft.AspNetCore.Mvc;
using SagaDemo.OrderService.Entities;
using SagaDemo.OrderService.Persistence.Users;

namespace SagaDemo.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        IReadOnlyList<User> users = await _userRepository.GetAllAsync();
        return Ok(users);
    }
}
