using Jira.Api.Middleware;
using Jira.Api.Extensions;

// CreateBuilder() => Creates the dependency injection container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

// Convert registration list into the actual Service Provider
var app = builder.Build();

// Configure middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// slow requests
app.UseMiddleware<RequestDelayMiddleware>();

app.UseExceptionHandler();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
