using System.Text;
using System.Text.Json.Serialization;

using Jira.Api.ExceptionHandling;
using Jira.Infrastructure;
using Jira.Infrastructure.Authentication;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Jira.Api.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Register Controllers
        _ = services.AddControllers().AddJsonOptions(options =>
                // Enum Serialization
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
        );

        // HTTP Logging
        _ = services.AddHttpLogging();

        // Register Infrastructure services
        _ = services.AddInfrastructure(configuration);

        // Register Global Exception Handler
        _ = services.AddExceptionHandler<GlobalExceptionHandler>();

        // Register Problem Details
        _ = services.AddProblemDetails();

        // JWT Authentication
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? throw new InvalidOperationException("Jwt settings are not configured.");

        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

        _ = services.AddAuthorization();

        return services;
    }
}
