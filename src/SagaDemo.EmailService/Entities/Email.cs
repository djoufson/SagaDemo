namespace SagaDemo.EmailService.Entities;

public class Email
{
    public Guid Id { get; set; }
    public string ToUserEmail { get; set; } = null!;
    public string Object { get; set; } = null!;
    public string Message { get; set; } = null!;
    public EmailState State { get; set; }
    public DateTime SentAt { get; internal set; }
}

public enum EmailState
{
    Pending,
    Sent,
    Fail
}
