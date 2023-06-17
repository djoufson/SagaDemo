using Microsoft.AspNetCore.Mvc;
using SagaDemo.AuthService.Dtos;
using SagaDemo.AuthService.Models;
using SagaDemo.AuthService.Services.Users;

namespace SagaDemo.AuthService.Controllers;

public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
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

        string? token = await _userService.LoginUserAsync(user.Email, user.Password);
        return Ok(new { user, token });
    }
}
