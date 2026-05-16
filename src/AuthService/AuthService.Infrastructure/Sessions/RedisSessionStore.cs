using System.Text.Json;
using AuthService.Application.Abstractions;
using AuthService.Application.Models;
using StackExchange.Redis;

namespace AuthService.Infrastructure.Sessions;

public sealed class RedisSessionStore : ISessionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDatabase _database;

    public RedisSessionStore(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task StoreSessionAsync(RefreshSession session, CancellationToken cancellationToken)
    {
        var refreshKey = BuildRefreshKey(session.RefreshTokenHash);
        var refreshTtl = session.RefreshExpiresAtUtc - DateTime.UtcNow;

        var payload = JsonSerializer.Serialize(session, JsonOptions);
        await _database.StringSetAsync(refreshKey, payload, refreshTtl);
        await _database.SetAddAsync(BuildUserRefreshSetKey(session.UserId), session.RefreshTokenHash);

        var accessKey = BuildAccessKey(session.AccessTokenJti);
        var accessTtl = session.AccessExpiresAtUtc - DateTime.UtcNow;
        await _database.StringSetAsync(accessKey, session.UserId.ToString(), accessTtl);
        await _database.SetAddAsync(BuildUserAccessSetKey(session.UserId), session.AccessTokenJti);
    }

    public async Task<RefreshSession?> GetSessionByRefreshHashAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        var value = await _database.StringGetAsync(BuildRefreshKey(refreshTokenHash));
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<RefreshSession>(value.ToString(), JsonOptions);
    }

    public Task<bool> IsAccessTokenActiveAsync(string accessJti, CancellationToken cancellationToken)
    {
        return _database.KeyExistsAsync(BuildAccessKey(accessJti));
    }

    public async Task RevokeSessionAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        var session = await GetSessionByRefreshHashAsync(refreshTokenHash, cancellationToken);
        if (session is null)
        {
            return;
        }

        await _database.KeyDeleteAsync(BuildRefreshKey(refreshTokenHash));
        await _database.KeyDeleteAsync(BuildAccessKey(session.AccessTokenJti));

        await _database.SetRemoveAsync(BuildUserRefreshSetKey(session.UserId), refreshTokenHash);
        await _database.SetRemoveAsync(BuildUserAccessSetKey(session.UserId), session.AccessTokenJti);
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var refreshSetKey = BuildUserRefreshSetKey(userId);
        var accessSetKey = BuildUserAccessSetKey(userId);

        var refreshHashes = await _database.SetMembersAsync(refreshSetKey);
        var accessJtis = await _database.SetMembersAsync(accessSetKey);

        if (refreshHashes.Length > 0)
        {
            var refreshKeys = refreshHashes.Select(x => (RedisKey)BuildRefreshKey(x!)).ToArray();
            await _database.KeyDeleteAsync(refreshKeys);
        }

        if (accessJtis.Length > 0)
        {
            var accessKeys = accessJtis.Select(x => (RedisKey)BuildAccessKey(x!)).ToArray();
            await _database.KeyDeleteAsync(accessKeys);
        }

        await _database.KeyDeleteAsync(refreshSetKey);
        await _database.KeyDeleteAsync(accessSetKey);
    }

    private static string BuildRefreshKey(string hash) => $"auth:refresh:{hash}";
    private static string BuildAccessKey(string jti) => $"auth:access:{jti}";
    private static string BuildUserRefreshSetKey(Guid userId) => $"auth:user:{userId}:refreshes";
    private static string BuildUserAccessSetKey(Guid userId) => $"auth:user:{userId}:accesses";
}

