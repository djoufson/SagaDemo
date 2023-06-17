namespace SagaDemo.EmailService.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public string EmailAddress { get; set; } = null!;
}
