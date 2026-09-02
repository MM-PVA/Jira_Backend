using Jira.Infrastructure.Persistence;
using Jira.Application.Authentication.Interfaces;
using Jira.Application.Workspaces.Interfaces;
using Jira.Application.Projects.Interfaces;
using Jira.Application.ProjectTasks.Interfaces;
using Jira.Infrastructure.Authentication;
using Jira.Infrastructure.Workspaces;
using Jira.Infrastructure.Projects;
using Jira.Infrastructure.ProjectTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Jira.Application.Logging.Interfaces;
using Jira.Infrastructure.Logging;
using Jira.Application.Authentication.Interfaces.Repositories;
using Jira.Application.Workspaces.Interfaces.Repositories;
using Jira.Application.Projects.Interfaces.Repositories;
using Jira.Application.ProjectTasks.Interfaces.Repositories;
// using Microsoft.Azure.Cosmos;

namespace Jira.Infrastructure;

public static class InfrastructureRegistration
{
    // IServiceCollection is a list of service registrations.
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // options is the object configures how EF Core should work.
        // services.AddDbContext<AppDbContext>(options =>
        // {
        //     options.UseCosmos(configuration.GetConnectionString("DefaultConnection")!, configuration["CosmosDb:DatabaseName"]!,
        //         cosmosOptions => cosmosOptions.ConnectionMode(ConnectionMode.Gateway));
        // });

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")
            );
        });

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.Configure<LoggingSettings>(configuration.GetSection(LoggingSettings.SectionName));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IWorkspaceService, WorkspaceService>();

        services.AddScoped<IProjectService, ProjectService>();

        services.AddScoped<IProjectTaskService, ProjectTaskService>();

        services.AddScoped<ILogService, LogService>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();

        services.AddScoped<IProjectRepository, ProjectRepository>();

        services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
