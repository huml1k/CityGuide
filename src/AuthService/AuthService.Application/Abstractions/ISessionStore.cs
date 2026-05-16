using AuthService.Application.Models;

namespace AuthService.Application.Abstractions;

public interface ISessionStore
{
    Task StoreSessionAsync(RefreshSession session, CancellationToken cancellationToken);
    Task<RefreshSession?> GetSessionByRefreshHashAsync(string refreshTokenHash, CancellationToken cancellationToken);
    Task<bool> IsAccessTokenActiveAsync(string accessJti, CancellationToken cancellationToken);
    Task RevokeSessionAsync(string refreshTokenHash, CancellationToken cancellationToken);
    Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken);
}

