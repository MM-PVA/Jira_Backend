using Jira.Domain.Common;

namespace Jira.Domain.Exceptions;

public class ConflictException : AppException
{
    public ConflictException()
        : base("A conflict occurred.")
    {
    }

    public ConflictException(string message)
    : base(message, ErrorCodes.Conflict)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, ErrorCodes.Conflict, innerException)
    {
    }
}
