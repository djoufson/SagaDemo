namespace SagaDemo.OrderService.Dtos;

public record PostOrderRequestDto(
    Guid ProductId,
    int Quantity
);
