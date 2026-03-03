using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceService.Api.Extenstions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("Некорректный JWT токен");
        }

        return userId;
    }
}