using AuthService.Api.Contracts;
using AuthService.Application.Abstractions;
using AuthService.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password, cancellationToken);
            return Created($"/api/auth/users/{result.UserId}", AuthResponse.FromResult(result));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
            return Ok(AuthResponse.FromResult(result));
        }
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RefreshAsync(request.RefreshToken, cancellationToken);
            return Ok(AuthResponse.FromResult(result));
        }
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.LogoutAsync(request.RefreshToken, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("introspect")]
    public async Task<IActionResult> Introspect([FromBody] IntrospectRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var payload = await _authService.IntrospectAsync(request.AccessToken, cancellationToken);
            return Ok(new IntrospectResponse(true, payload.UserId, payload.Email, payload.Role, payload.ExpiresAtUtc));
        }
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var authorization = Request.Headers.Authorization.ToString();
        var accessToken = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization["Bearer ".Length..].Trim()
            : null;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized();
        }

        try
        {
            var payload = await _authService.IntrospectAsync(accessToken, cancellationToken);
            return Ok(new IntrospectResponse(true, payload.UserId, payload.Email, payload.Role, payload.ExpiresAtUtc));
        }
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
    }
}

