using System.Text.Json.Serialization;

using FluentValidation;
using FluentValidation.AspNetCore;

using Jira.Application.ProjectTasks.Validators;
using Jira.Api.ExceptionHandling;
using Jira.Infrastructure;

using Asp.Versioning;

namespace Jira.Api.Extensions;

internal static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Register Controllers
        _ = services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Register Fluent Validation
        _ = services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);

        // Register Validators
        _ = services.AddValidatorsFromAssemblyContaining<CreateProjectTaskRequestValidator>();

        // Add Versioning
        _ = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                // options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "v";
                options.SubstituteApiVersionInUrl = true;
            });

        // HTTP Logging
        _ = services.AddHttpLogging();

        // Register Infrastructure services
        _ = services.AddInfrastructure(configuration);

        // Register Global Exception Handler
        _ = services.AddExceptionHandler<GlobalExceptionHandler>();

        // Register Problem Details
        _ = services.AddProblemDetails();

        _ = services.AddJwtAuthentication(configuration);

        return services;
    }
}
