namespace ContentService.Application.Common.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("User is unauthorized.")
        {
        }

        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
