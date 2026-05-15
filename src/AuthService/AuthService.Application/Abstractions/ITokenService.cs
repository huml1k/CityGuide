using AuthService.Application.Models;
using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions;

public interface ITokenService
{
    TokenPair GenerateTokenPair(User user);
    bool TryValidateAccessToken(string token, out AccessTokenPayload payload);
}

