using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Abstractions;
using AuthService.Application.Exceptions;
using AuthService.Application.Models;
using AuthService.Domain.Entities;

namespace AuthService.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ISessionStore _sessionStore;

    public AuthService(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ISessionStore sessionStore)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _sessionStore = sessionStore;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        ValidateRegistrationInput(normalizedEmail, password);

        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("Email is already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(password),
            Role = AuthRole.User,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userProfileRepository.AddAsync(new UserProfile { UserId = user.Id }, cancellationToken);

        await _userRepository.SaveChangesAsync(cancellationToken);
        await _userProfileRepository.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(user, revokeExistingSessions: true, cancellationToken);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        ValidateLoginInput(normalizedEmail, password);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new BadRequestException("Invalid email or password.");
        }

        return await IssueTokensAsync(user, revokeExistingSessions: true, cancellationToken);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var refreshHash = HashToken(refreshToken);
        var session = await _sessionStore.GetSessionByRefreshHashAsync(refreshHash, cancellationToken);

        if (session is null || session.RefreshExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token is invalid or expired.");
        }

        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedException("User not found.");
        }

        await _sessionStore.RevokeSessionAsync(refreshHash, cancellationToken);
        return await IssueTokensAsync(user, revokeExistingSessions: false, cancellationToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var refreshHash = HashToken(refreshToken);
        var session = await _sessionStore.GetSessionByRefreshHashAsync(refreshHash, cancellationToken);

        if (session is null)
        {
            throw new UnauthorizedException("Refresh token is invalid.");
        }

        await _sessionStore.RevokeSessionAsync(refreshHash, cancellationToken);
    }

    public async Task<AccessTokenPayload> IntrospectAsync(string accessToken, CancellationToken cancellationToken)
    {
        if (!_tokenService.TryValidateAccessToken(accessToken, out var payload))
        {
            throw new UnauthorizedException("Access token is invalid.");
        }

        var isActive = await _sessionStore.IsAccessTokenActiveAsync(payload.Jti, cancellationToken);
        if (!isActive)
        {
            throw new UnauthorizedException("Access token is revoked.");
        }

        return payload;
    }

    private async Task<AuthResult> IssueTokensAsync(
        User user,
        bool revokeExistingSessions,
        CancellationToken cancellationToken)
    {
        if (revokeExistingSessions)
        {
            await _sessionStore.RevokeAllUserSessionsAsync(user.Id, cancellationToken);
        }

        var tokenPair = _tokenService.GenerateTokenPair(user);
        var refreshHash = HashToken(tokenPair.RefreshToken);

        var session = new RefreshSession(
            user.Id,
            user.Email,
            user.Role,
            refreshHash,
            tokenPair.RefreshTokenExpiresAtUtc,
            tokenPair.AccessTokenJti,
            tokenPair.AccessTokenExpiresAtUtc);

        await _sessionStore.StoreSessionAsync(session, cancellationToken);

        return new AuthResult(
            user.Id,
            user.Email,
            user.Role,
            tokenPair.AccessToken,
            tokenPair.AccessTokenExpiresAtUtc,
            tokenPair.RefreshToken,
            tokenPair.RefreshTokenExpiresAtUtc);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static void ValidateLoginInput(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new BadRequestException("Email is invalid.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new BadRequestException("Password is required.");
        }
    }

    private static void ValidateRegistrationInput(string email, string password)
    {
        ValidateLoginInput(email, password);

        if (password.Length < 8)
        {
            throw new BadRequestException("Password must be at least 8 characters.");
        }
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

