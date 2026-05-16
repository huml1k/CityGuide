using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Abstractions;
using AuthService.Application.Models;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly AuthOptions _options;
    private readonly byte[] _secret;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenService(IOptions<AuthOptions> options)
    {
        _options = options.Value;
        _secret = Encoding.UTF8.GetBytes(_options.JwtSecret);
    }

    public TokenPair GenerateTokenPair(User user)
    {
        var now = DateTime.UtcNow;
        var accessExpires = now.AddMinutes(_options.AccessTokenMinutes);
        var refreshExpires = now.AddDays(_options.RefreshTokenDays);
        var jti = Guid.NewGuid().ToString("N");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        var token = new JwtSecurityToken(
            issuer: _options.JwtIssuer,
            audience: _options.JwtAudience,
            claims: claims,
            notBefore: now,
            expires: accessExpires,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256));

        var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)).ToLowerInvariant();

        return new TokenPair(
            _handler.WriteToken(token),
            accessExpires,
            jti,
            refreshToken,
            refreshExpires);
    }

    public bool TryValidateAccessToken(string token, out AccessTokenPayload payload)
    {
        payload = default!;

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = _options.JwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secret),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _handler.ValidateToken(token, validationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtToken)
            {
                return false;
            }

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "User";
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var exp = jwtToken.ValidTo;

            if (!Guid.TryParse(sub, out var userId) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(jti))
            {
                return false;
            }

            payload = new AccessTokenPayload(userId, email, role, jti, exp);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

