using Microsoft.AspNetCore.Mvc;
using SagaDemo.PaymentService.Dtos;
using SagaDemo.PaymentService.Entities;
using SagaDemo.PaymentService.Services.Payments;

namespace SagaDemo.PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> InitiatePayment(TransactionDto transactionDto)
    {
        Transaction transaction = await _paymentService.MakeTransactionAsync(new Transaction()
        {
            OrderId = transactionDto.OrderId,
            State = TransactionState.Success,
            PurchaseDate = DateTime.Now
        });

        return Ok(transaction);
    }
}
