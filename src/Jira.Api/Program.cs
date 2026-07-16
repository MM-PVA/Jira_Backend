using System.Text;
using Jira.Api.ExceptionHandling;
using Jira.Api.Middleware;
using Jira.Infrastructure;
using Jira.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

// CreateBuilder() => Creates the dependency injection container.
var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // Enum Serialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); 
    
builder.Services.AddHttpLogging();

// Register Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Register Global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// To follow the standard common JSON structure called Problem Details.
builder.Services.AddProblemDetails();

// JWT Authentication
// Here .Get<JwtSettings>() is Configuration Binding which binds JSON configuration to class.
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt settings are not configured.");

// JwtBearerDefaults.AuthenticationScheme = Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

// Convert registration list into the actual Service Provider
var app = builder.Build();

// Configure middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseExceptionHandler();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("\nJira api server is running on http://localhost:5073");
app.Run();