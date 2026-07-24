using Jira.Domain.Exceptions;
using Jira.Application.AppErrors;

using Microsoft.AspNetCore.Diagnostics;

namespace Jira.Api.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var (statusCode, title) = exception switch
        {
            ConflictException => (409, "Conflict"),
            NotFoundException => (404, "Not Found"),
            UnauthorizedException => (401, "Unauthorized"),
            _ => (500, "Internal Server Error")
        };

        var errorCode = exception is AppException appException ? appException.ErrorCode : 5000;

        var response = new ErrorResponse(
            Status: statusCode,
            Code: errorCode,
            Title: title,
            Message: exception.Message,
            TraceId: httpContext.TraceIdentifier
        );

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);

        return true;
    }
}
