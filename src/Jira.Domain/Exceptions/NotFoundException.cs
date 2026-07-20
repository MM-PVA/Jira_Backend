using Jira.Domain.Common;

namespace Jira.Domain.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException()
        : base("Resource not found.", ErrorCodes.NotFound)
    {
    }

    public NotFoundException(string message)
        : base(message, ErrorCodes.NotFound)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, ErrorCodes.NotFound, innerException)
    {
    }
}
