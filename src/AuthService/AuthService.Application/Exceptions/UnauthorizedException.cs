namespace AuthService.Application.Exceptions;

public sealed class UnauthorizedException : AuthException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}

