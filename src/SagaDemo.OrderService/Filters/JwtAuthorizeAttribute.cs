using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using SagaDemo.OrderService.Configurations;

namespace SagaDemo.OrderService.Attributes;

public class JwtAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if(authHeader is null || !authHeader.StartsWith("Bearer"))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        using var scope = context.HttpContext.RequestServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<JwtAuthorizeAttribute>>();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
        var token = authHeader["Bearer ".Length..];
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
        try
        {
            var claimsPrincipals = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validToken);
            string? email = claimsPrincipals.FindFirst(ClaimTypes.Email)?.Value;
            if(email is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            context.HttpContext.Request.Headers.Add(HeaderKeys.EmailHeaderKey, email);
        }
        catch (Exception e)
        {
            context.Result = new UnauthorizedResult();
            logger.LogError(e, "Error while attempting to authenticate the user");
        }
    }
}
