using SagaDemo.PaymentService.Events;

namespace SagaDemo.PaymentService.Commands;

public class UndoMakePaymentCommand : Event
{
    public Guid OrderId { get; set; }
    public override string EventName => EventType;
    public const string EventType = nameof(UndoMakePaymentCommand);
}
