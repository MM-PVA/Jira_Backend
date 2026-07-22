namespace Jira.Application.AppErrors;

public sealed record ErrorResponse(
    int Status,
    int Code,
    string Title,
    string Message,
    string TraceId
);
