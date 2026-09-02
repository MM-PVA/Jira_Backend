// using Jira.Domain.Entities;

// using Microsoft.EntityFrameworkCore;

// namespace Jira.Infrastructure.Persistence;

// public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
// {
//     // Convert enums to strings for storage in the database
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         ArgumentNullException.ThrowIfNull(modelBuilder);

//         base.OnModelCreating(modelBuilder);

//         // User
//         // Store User entities in the Cosmos DB container named "Users"
//         _ = modelBuilder.Entity<User>().ToContainer("Users");
//         // User.Id is the primary key of the User entity
//         _ = modelBuilder.Entity<User>().HasKey(user => user.Id);
//         // When serializing User.Id into the Cosmos JSON document, call the JSON property "id"
//         _ = modelBuilder.Entity<User>().Property(user => user.Id).ToJsonProperty("id");
//         // Use User.Id as the partition key
//         _ = modelBuilder.Entity<User>().HasPartitionKey(user => user.Id);
//         // Do not map the OwnedWorkspaces property
//         _ = modelBuilder.Entity<User>().Ignore(user => user.OwnedWorkspaces);
//         // Do not map the AssignedTasks property
//         _ = modelBuilder.Entity<User>().Ignore(user => user.AssignedTasks);

//         // Workspace
//         _ = modelBuilder.Entity<Workspace>().ToContainer("Workspaces");
//         _ = modelBuilder.Entity<Workspace>().HasKey(workspace => workspace.Id);
//         _ = modelBuilder.Entity<Workspace>().Property(workspace => workspace.Id).ToJsonProperty("id");
//         _ = modelBuilder.Entity<Workspace>().HasPartitionKey(workspace => workspace.OwnerId);
//         _ = modelBuilder.Entity<Workspace>().Ignore(workspace => workspace.Owner);
//         _ = modelBuilder.Entity<Workspace>().Ignore(workspace => workspace.Projects);

//         // Project
//         _ = modelBuilder.Entity<Project>().ToContainer("Projects");
//         _ = modelBuilder.Entity<Project>().HasKey(project => project.Id);
//         _ = modelBuilder.Entity<Project>().Property(project => project.Id).ToJsonProperty("id");
//         _ = modelBuilder.Entity<Project>().HasPartitionKey(project => project.WorkspaceId);
//         _ = modelBuilder.Entity<Project>().Ignore(project => project.Workspace);
//         _ = modelBuilder.Entity<Project>().Ignore(project => project.Tasks);

//         // ProjectTask
//         _ = modelBuilder.Entity<ProjectTask>().ToContainer("ProjectTasks");
//         _ = modelBuilder.Entity<ProjectTask>().HasKey(task => task.Id);
//         _ = modelBuilder.Entity<ProjectTask>().Property(task => task.Id).ToJsonProperty("id");
//         _ = modelBuilder.Entity<ProjectTask>().HasPartitionKey(task => task.ProjectId);
//         _ = modelBuilder.Entity<ProjectTask>().Ignore(task => task.Project);

//         // RefreshToken
//         _ = modelBuilder.Entity<RefreshToken>().ToContainer("RefreshTokens");
//         _ = modelBuilder.Entity<RefreshToken>().HasKey(refreshToken => refreshToken.Id);
//         _ = modelBuilder.Entity<RefreshToken>().Property(refreshToken => refreshToken.Id).ToJsonProperty("id");
//         _ = modelBuilder.Entity<RefreshToken>().HasPartitionKey(refreshToken => refreshToken.UserId);
//         _ = modelBuilder.Entity<RefreshToken>().Ignore(refreshToken => refreshToken.IsRevoked);
//         _ = modelBuilder.Entity<RefreshToken>().Ignore(refreshToken => refreshToken.IsExpired);

//         // Enum conversions
//         _ = modelBuilder.Entity<ProjectTask>().Property(task => task.Status).HasConversion<string>();
//         _ = modelBuilder.Entity<ProjectTask>().Property(task => task.Priority).HasConversion<string>();
//     }

//     public DbSet<User> Users => Set<User>();
//     public DbSet<Workspace> Workspaces => Set<Workspace>();
//     public DbSet<Project> Projects => Set<Project>();
//     public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
//     public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
// }

using Jira.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(user => user.PasswordHash)
                .IsRequired();
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.ToTable("Workspaces");

            entity.HasKey(workspace => workspace.Id);

            entity.Property(workspace => workspace.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(workspace => workspace.Description)
                .HasMaxLength(500);

            entity.HasOne(workspace => workspace.Owner)
                .WithMany(user => user.OwnedWorkspaces)
                .HasForeignKey(workspace => workspace.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");

            entity.HasKey(project => project.Id);

            entity.Property(project => project.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(project => project.Description)
                .HasMaxLength(500);

            entity.HasOne(project => project.Workspace)
                .WithMany(workspace => workspace.Projects)
                .HasForeignKey(project => project.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.ToTable("ProjectTasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Title)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(task => task.Description)
                .HasMaxLength(1000);

            entity.Property(task => task.Status)
                .HasConversion<string>();

            entity.Property(task => task.Priority)
                .HasConversion<string>();

            // Keep the application property name AssigneeId,
            // but persist it to the existing Supabase column UserId.
            entity.Property(task => task.AssigneeId)
                .HasColumnName("UserId");

            entity.HasOne<User>()
                .WithMany(user => user.AssignedTasks)
                .HasForeignKey(task => task.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(task => task.Project)
                .WithMany(project => project.Tasks)
                .HasForeignKey(task => task.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(refreshToken => refreshToken.Id);

            entity.Property(refreshToken => refreshToken.TokenHash)
                .IsRequired();

            entity.Property(refreshToken => refreshToken.ExpiresAtUtc)
                .IsRequired();
        });
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
