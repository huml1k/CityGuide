namespace AuthService.Application.Exceptions;

public sealed class BadRequestException : AuthException
{
    public BadRequestException(string message) : base(message)
    {
    }
}

