using System.Security.Claims;

namespace SagaDemo.AuthService.Services.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(IEnumerable<Claim> claims);
}
