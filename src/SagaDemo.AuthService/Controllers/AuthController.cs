using Microsoft.AspNetCore.Mvc;
using SagaDemo.AuthService.Contracts;
using SagaDemo.AuthService.Dtos;
using SagaDemo.AuthService.Models;
using SagaDemo.AuthService.Services.RabbitMq;
using SagaDemo.AuthService.Services.Users;

namespace SagaDemo.AuthService.Controllers;

public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBroadcastClient _broadcastClient;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        IBroadcastClient broadcastClient,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _broadcastClient = broadcastClient;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        string? token = await _userService.LoginUserAsync(loginDto.Email, loginDto.Password);
        if(string.IsNullOrEmpty(token))
            return BadRequest("Wrong credentials");

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        bool exists = await _userService.ExistsAsync(registerDto.Email);
        if(exists)
            return BadRequest("The specified email address is already assigned to an account");

        User user = new()
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        _ = await _userService.RegisterUserAsync(user);

        try
        {
            await _broadcastClient.PublishUserRegisteredAsync(new UserRegistered()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to publish the event");
        }
        string? token = await _userService.LoginUserAsync(user.Email, user.Password);
        return Ok(new { user, token });
    }
}
