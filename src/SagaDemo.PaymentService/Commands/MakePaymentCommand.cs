using SagaDemo.PaymentService.Events;

namespace SagaDemo.PaymentService.Commands;

public class MakePaymentCommand : Event
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(MakePaymentCommand);
}
