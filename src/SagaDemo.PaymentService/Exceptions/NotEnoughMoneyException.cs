namespace SagaDemo.PaymentService.Exceptions;

public class NotEnoughMoneyException : Exception
{
    public override string Message => "The amount of the transaction exceed your account current balance";
}
