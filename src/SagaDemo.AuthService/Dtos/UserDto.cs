namespace SagaDemo.AuthService.Dtos;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Password
);
