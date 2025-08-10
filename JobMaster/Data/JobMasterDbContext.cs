using Microsoft.EntityFrameworkCore;
using JobMaster.Models.GitHub;
using JobMaster.Models.Jira;

namespace JobMaster.Data;

public class JobMasterDbContext : DbContext
{
    public JobMasterDbContext(DbContextOptions<JobMasterDbContext> options) : base(options)
    {
    }

    // GitHub DbSets
    public DbSet<GitHubOrganisation> GitHubOrganisations { get; set; }
    public DbSet<GitHubRepository> GitHubRepositories { get; set; }
    public DbSet<GitHubWorkflow> GitHubWorkflows { get; set; }
    public DbSet<GitHubWorkflowRun> GitHubWorkflowRuns { get; set; }
    public DbSet<GitHubWorkflowJob> GitHubWorkflowJobs { get; set; }

    // Jira DbSets
    public DbSet<JiraIssue> JiraIssues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure GitHub Organisation
        modelBuilder.Entity<GitHubOrganisation>(entity =>
        {
            entity.HasIndex(e => e.GitHubId).IsUnique();
            entity.HasIndex(e => e.Login);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });

        // Configure GitHub Repository
        modelBuilder.Entity<GitHubRepository>(entity =>
        {
            entity.HasIndex(e => e.GitHubId).IsUnique();
            entity.HasIndex(e => e.FullName);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.HasOne(d => d.Organisation)
                .WithMany(p => p.Repositories)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure GitHub Workflow
        modelBuilder.Entity<GitHubWorkflow>(entity =>
        {
            entity.HasIndex(e => e.GitHubId).IsUnique();
            entity.HasIndex(e => new { e.RepositoryId, e.Name });
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.HasOne(d => d.Repository)
                .WithMany(p => p.Workflows)
                .HasForeignKey(d => d.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GitHub Workflow Run
        modelBuilder.Entity<GitHubWorkflowRun>(entity =>
        {
            entity.HasIndex(e => e.GitHubId).IsUnique();
            entity.HasIndex(e => new { e.WorkflowId, e.RunNumber });
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.HasOne(d => d.Workflow)
                .WithMany(p => p.WorkflowRuns)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GitHub Workflow Job
        modelBuilder.Entity<GitHubWorkflowJob>(entity =>
        {
            entity.HasIndex(e => e.GitHubId).IsUnique();
            entity.HasIndex(e => new { e.WorkflowRunId, e.Name });
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.HasOne(d => d.WorkflowRun)
                .WithMany(p => p.WorkflowJobs)
                .HasForeignKey(d => d.WorkflowRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Jira Issue
        modelBuilder.Entity<JiraIssue>(entity =>
        {
            entity.HasIndex(e => e.IssueKey).IsUnique();
            entity.HasIndex(e => e.IssueId);
            entity.HasIndex(e => new { e.ProjectKey, e.Status });
            entity.HasIndex(e => e.JiraUpdatedAt);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });
    }
}
