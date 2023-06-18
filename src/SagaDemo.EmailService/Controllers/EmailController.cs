using Microsoft.AspNetCore.Mvc;
using SagaDemo.EmailService.Dtos;
using SagaDemo.EmailService.Entities;
using SagaDemo.EmailService.Events;
using SagaDemo.EmailService.Services.Emails;
using SagaDemo.EmailService.Services.Orchestrator;

namespace SagaDemo.EmailService.Controllers;

public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IOrchestratorClient _orchestrator;

    public EmailController(IEmailService emailService, IOrchestratorClient orchestrator)
    {
        _emailService = emailService;
        _orchestrator = orchestrator;
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailDto emailDto)
    {
        Email email = new()
        {
            ToUserEmail = emailDto.ToUserEmail,
            Object = emailDto.Object,
            Message = emailDto.Message,
            State = EmailState.Pending
        };
        bool success = await _emailService.SendAsync(email);
        if(success)
        {
            await _orchestrator.RaiseEmailSentEvent(new EmailSent()
            {
                EmailId = email.Id,
            });
        }
        else
        {
            await _orchestrator.RaiseEmailFailedEvent(new EmailFailed()
            {
                
            });
        }
        return Ok(email.State);
    }
}
