using Jira.Api.Extensions;
using Jira.Api.Middleware;
using Jira.Infrastructure.Persistence;

// CreateBuilder() => Creates the dependency injection container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

// Convert registration list into the actual Service Provider
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
}

// Configure middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// slow requests
// app.UseMiddleware<RequestDelayMiddleware>();

app.UseExceptionHandler();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
