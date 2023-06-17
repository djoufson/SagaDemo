using Microsoft.AspNetCore.Mvc;
using SagaDemo.PaymentService.Entities;

namespace SagaDemo.PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    public IActionResult InitiatePayment(Transaction transaction)
    {
        return Ok(transaction);
    }
}
