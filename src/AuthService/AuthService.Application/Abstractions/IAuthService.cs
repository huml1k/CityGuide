using AuthService.Application.Models;

namespace AuthService.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken);
    Task<AccessTokenPayload> IntrospectAsync(string accessToken, CancellationToken cancellationToken);
}

