using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services;

public class JwtTokenService(IOptions<JwtSettings> jwtSettings) : IJwtTokenService
{
    public string CreateToken(User user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(jwtSettings.Value.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }
}