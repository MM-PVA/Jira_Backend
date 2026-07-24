namespace Jira.Api.Middleware;

public sealed class RequestDelayMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    private readonly int _requestDelayInSeconds = 1;

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await Task.Delay(TimeSpan.FromSeconds(_requestDelayInSeconds), context.RequestAborted).ConfigureAwait(false);

        await _next(context).ConfigureAwait(false);
    }
}
