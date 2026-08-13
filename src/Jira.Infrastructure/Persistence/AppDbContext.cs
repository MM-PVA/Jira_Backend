using Jira.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Convert enums to strings for storage in the database
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        // User
        // Store User entities in the Cosmos DB container named "Users"
        _ = modelBuilder.Entity<User>().ToContainer("Users");
        // User.Id is the primary key of the User entity
        _ = modelBuilder.Entity<User>().HasKey(user => user.Id);
        // When serializing User.Id into the Cosmos JSON document, call the JSON property "id"
        _ = modelBuilder.Entity<User>().Property(user => user.Id).ToJsonProperty("id");
        // Use User.Id as the partition key
        _ = modelBuilder.Entity<User>().HasPartitionKey(user => user.Id);
        // Do not map the OwnedWorkspaces property
        _ = modelBuilder.Entity<User>().Ignore(user => user.OwnedWorkspaces);
        // Do not map the AssignedTasks property
        _ = modelBuilder.Entity<User>().Ignore(user => user.AssignedTasks);

        // Workspace
        _ = modelBuilder.Entity<Workspace>().ToContainer("Workspaces");
        _ = modelBuilder.Entity<Workspace>().HasKey(workspace => workspace.Id);
        _ = modelBuilder.Entity<Workspace>().Property(workspace => workspace.Id).ToJsonProperty("id");
        _ = modelBuilder.Entity<Workspace>().HasPartitionKey(workspace => workspace.OwnerId);
        _ = modelBuilder.Entity<Workspace>().Ignore(workspace => workspace.Owner);
        _ = modelBuilder.Entity<Workspace>().Ignore(workspace => workspace.Projects);

        // Project
        _ = modelBuilder.Entity<Project>().ToContainer("Projects");
        _ = modelBuilder.Entity<Project>().HasKey(project => project.Id);
        _ = modelBuilder.Entity<Project>().Property(project => project.Id).ToJsonProperty("id");
        _ = modelBuilder.Entity<Project>().HasPartitionKey(project => project.WorkspaceId);
        _ = modelBuilder.Entity<Project>().Ignore(project => project.Workspace);
        _ = modelBuilder.Entity<Project>().Ignore(project => project.Tasks);

        // ProjectTask
        _ = modelBuilder.Entity<ProjectTask>().ToContainer("ProjectTasks");
        _ = modelBuilder.Entity<ProjectTask>().HasKey(task => task.Id);
        _ = modelBuilder.Entity<ProjectTask>().Property(task => task.Id).ToJsonProperty("id");
        _ = modelBuilder.Entity<ProjectTask>().HasPartitionKey(task => task.ProjectId);
        _ = modelBuilder.Entity<ProjectTask>().Ignore(task => task.Project);

        // RefreshToken
        _ = modelBuilder.Entity<RefreshToken>().ToContainer("RefreshTokens");
        _ = modelBuilder.Entity<RefreshToken>().HasKey(refreshToken => refreshToken.Id);
        _ = modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.Id).ToJsonProperty("id");
        _ = modelBuilder.Entity<RefreshToken>().HasPartitionKey(refreshToken => refreshToken.UserId);
        _ = modelBuilder.Entity<RefreshToken>().Ignore(refreshToken => refreshToken.IsRevoked);
        _ = modelBuilder.Entity<RefreshToken>().Ignore(refreshToken => refreshToken.IsExpired);

        // Enum conversions
        _ = modelBuilder.Entity<ProjectTask>().Property(task => task.Status).HasConversion<string>();
        _ = modelBuilder.Entity<ProjectTask>().Property(task => task.Priority).HasConversion<string>();
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
