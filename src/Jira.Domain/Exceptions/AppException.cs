namespace Jira.Domain.Exceptions;

public abstract class AppException : Exception
{
    protected AppException()
    {
    }

    protected AppException(string message)
        : base(message)
    {
    }

    protected AppException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected AppException(string message, int errorCode)
        : base(message)
        => ErrorCode = errorCode;

    protected AppException(string message, int errorCode, Exception innerException)
        : base(message, innerException)
        => ErrorCode = errorCode;

    public int ErrorCode { get; }
}
