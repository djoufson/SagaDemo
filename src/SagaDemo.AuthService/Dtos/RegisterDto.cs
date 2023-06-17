namespace SagaDemo.AuthService.Dtos;
public record RegisterDto(
    string Name,
    string Email,
    string Password
);
