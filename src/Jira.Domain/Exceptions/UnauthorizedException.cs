using Jira.Domain.Common;

namespace Jira.Domain.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException()
        : base("Unauthorized access.")
    {
    }

    public UnauthorizedException(string message)
        : base(message, ErrorCodes.Unauthorized)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, ErrorCodes.Unauthorized, innerException)
    {
    }
}
