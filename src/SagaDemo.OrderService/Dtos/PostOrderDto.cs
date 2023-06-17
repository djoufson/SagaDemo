namespace SagaDemo.OrderService.Dtos;

public record PostOrderDto(
    Guid ProductId,
    Guid UserId,
    int Quantity
);
