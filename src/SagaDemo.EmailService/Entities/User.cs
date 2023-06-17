namespace SagaDemo.EmailService.Entities;

public class User
{
    public Guid Id { get; set; }
    public string EmailAddress { get; set; } = null!;
}
