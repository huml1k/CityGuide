namespace AuthService.Application.Exceptions;

public sealed class ConflictException : AuthException
{
    public ConflictException(string message) : base(message)
    {
    }
}

