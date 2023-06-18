namespace SagaDemo.EmailService.Dtos;

public record EmailDto(
    string ToUserEmail,
    string Object,
    string Message
);
