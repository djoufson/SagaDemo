namespace SagaDemo.AuthService.Configurations;

public sealed class JwtSettings
{
    public static string SectionName => nameof(JwtSettings);
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpirationInDays { get; init; }
    public string SecretKey { get; init; } = null!;
}
