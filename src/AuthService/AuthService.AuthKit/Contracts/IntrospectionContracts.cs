namespace AuthService.AuthKit.Contracts;

public sealed record IntrospectRequest(string AccessToken);

public sealed record IntrospectResponse(
    bool IsValid,
    Guid UserId,
    string Email,
    string Role,
    DateTime ExpiresAtUtc);

