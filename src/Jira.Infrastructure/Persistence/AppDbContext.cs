using Jira.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Jira.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Convert enums to strings for storage in the database
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProjectTask>().Property(task => task.Status).HasConversion<string>();
        modelBuilder.Entity<ProjectTask>().Property(task => task.Priority).HasConversion<string>();
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
}
