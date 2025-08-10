using Hangfire;
using JobMaster.Data;
using JobMaster.Models.GitHub;
using JobMaster.Services;
using Microsoft.EntityFrameworkCore;

namespace JobMaster.Jobs;

public class GitHubSyncJob
{
    private readonly JobMasterDbContext _context;
    private readonly GitHubService _gitHubService;
    private readonly ILogger<GitHubSyncJob> _logger;
    private readonly IConfiguration _configuration;

    public GitHubSyncJob(
        JobMasterDbContext context,
        GitHubService gitHubService,
        ILogger<GitHubSyncJob> logger,
        IConfiguration configuration)
    {
        _context = context;
        _gitHubService = gitHubService;
        _logger = logger;
        _configuration = configuration;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SyncAsync()
    {
        var token = _configuration["GitHub:Token"];
        _logger.LogInformation("Starting GitHub sync job at {Time}", DateTime.UtcNow);

        try
        {
            // Sync organisations
            await SyncOrganisationsAsync(token);
            
            _logger.LogInformation("GitHub sync job completed successfully at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GitHub sync job failed");
            throw;
        }
    }

    private async Task SyncOrganisationsAsync(string? token)
    {
        _logger.LogInformation("Syncing GitHub organisations");
        
        var organisations = await _gitHubService.GetOrganisationsAsync(token);
        
        foreach (var orgDto in organisations)
        {
            // Check if organisation already exists
            var existingOrg = await _context.GitHubOrganisations
                .FirstOrDefaultAsync(o => o.GitHubId == orgDto.GitHubId);

            if (existingOrg != null)
            {
                // Update existing organisation
                existingOrg.Name = orgDto.Name;
                existingOrg.Description = orgDto.Description;
                existingOrg.Company = orgDto.Company;
                existingOrg.Blog = orgDto.Blog;
                existingOrg.Location = orgDto.Location;
                existingOrg.Email = orgDto.Email;
                existingOrg.TwitterUsername = orgDto.TwitterUsername;
                existingOrg.PublicRepos = orgDto.PublicRepos;
                existingOrg.PublicGists = orgDto.PublicGists;
                existingOrg.Followers = orgDto.Followers;
                existingOrg.Following = orgDto.Following;
                existingOrg.HtmlUrl = orgDto.HtmlUrl;
                existingOrg.AvatarUrl = orgDto.AvatarUrl;
                existingOrg.GitHubUpdatedAt = orgDto.GitHubUpdatedAt;
                existingOrg.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new organisation
                _context.GitHubOrganisations.Add(orgDto);
            }

            await _context.SaveChangesAsync();

            // Sync repositories for this organisation
            await SyncRepositoriesAsync(orgDto.Login, orgDto.Id, token);
        }
    }

    private async Task SyncRepositoriesAsync(string orgLogin, int orgId, string? token)
    {
        _logger.LogInformation("Syncing repositories for organisation: {OrgLogin}", orgLogin);
        
        var repositories = await _gitHubService.GetRepositoriesAsync(orgLogin, token);
        
        foreach (var repoDto in repositories)
        {
            // Check if repository already exists
            var existingRepo = await _context.GitHubRepositories
                .FirstOrDefaultAsync(r => r.GitHubId == repoDto.GitHubId);

            if (existingRepo != null)
            {
                // Update existing repository
                UpdateRepository(existingRepo, repoDto, orgId);
            }
            else
            {
                // Add new repository
                repoDto.OrganisationId = orgId;
                _context.GitHubRepositories.Add(repoDto);
            }

            await _context.SaveChangesAsync();

            // Sync workflows for this repository
            await SyncWorkflowsAsync(orgLogin, repoDto.Name, repoDto.Id, token);
        }
    }

    private async Task SyncWorkflowsAsync(string owner, string repoName, int repoId, string? token)
    {
        _logger.LogInformation("Syncing workflows for repository: {Owner}/{RepoName}", owner, repoName);
        
        var workflows = await _gitHubService.GetWorkflowsAsync(owner, repoName, token);
        
        foreach (var workflowDto in workflows)
        {
            // Check if workflow already exists
            var existingWorkflow = await _context.GitHubWorkflows
                .FirstOrDefaultAsync(w => w.GitHubId == workflowDto.GitHubId);

            if (existingWorkflow != null)
            {
                // Update existing workflow
                UpdateWorkflow(existingWorkflow, workflowDto);
            }
            else
            {
                // Add new workflow
                workflowDto.RepositoryId = repoId;
                _context.GitHubWorkflows.Add(workflowDto);
            }

            await _context.SaveChangesAsync();

            // Sync workflow runs for this workflow
            await SyncWorkflowRunsAsync(owner, repoName, workflowDto.GitHubId, workflowDto.Id, token);
        }
    }

    private async Task SyncWorkflowRunsAsync(string owner, string repoName, long workflowGitHubId, int workflowId, string? token)
    {
        _logger.LogInformation("Syncing workflow runs for workflow: {Owner}/{RepoName}/{WorkflowId}", owner, repoName, workflowGitHubId);
        
        var workflowRuns = await _gitHubService.GetWorkflowRunsAsync(owner, repoName, workflowGitHubId, token);
        
        foreach (var runDto in workflowRuns)
        {
            // Check if workflow run already exists
            var existingRun = await _context.GitHubWorkflowRuns
                .FirstOrDefaultAsync(r => r.GitHubId == runDto.GitHubId);

            if (existingRun != null)
            {
                // Update existing workflow run
                UpdateWorkflowRun(existingRun, runDto);
            }
            else
            {
                // Add new workflow run
                runDto.WorkflowId = workflowId;
                _context.GitHubWorkflowRuns.Add(runDto);
            }

            await _context.SaveChangesAsync();

            // Sync workflow jobs for this run
            await SyncWorkflowJobsAsync(owner, repoName, runDto.GitHubId, runDto.Id, token);
        }
    }

    private async Task SyncWorkflowJobsAsync(string owner, string repoName, long runGitHubId, int runId, string? token)
    {
        _logger.LogInformation("Syncing workflow jobs for run: {Owner}/{RepoName}/{RunId}", owner, repoName, runGitHubId);
        
        var workflowJobs = await _gitHubService.GetWorkflowJobsAsync(owner, repoName, runGitHubId, token);
        
        foreach (var jobDto in workflowJobs)
        {
            // Check if workflow job already exists
            var existingJob = await _context.GitHubWorkflowJobs
                .FirstOrDefaultAsync(j => j.GitHubId == jobDto.GitHubId);

            if (existingJob != null)
            {
                // Update existing workflow job
                UpdateWorkflowJob(existingJob, jobDto);
            }
            else
            {
                // Add new workflow job
                jobDto.WorkflowRunId = runId;
                _context.GitHubWorkflowJobs.Add(jobDto);
            }
        }

        await _context.SaveChangesAsync();
    }

    private static void UpdateRepository(GitHubRepository existing, GitHubRepository updated, int orgId)
    {
        existing.Name = updated.Name;
        existing.FullName = updated.FullName;
        existing.Description = updated.Description;
        existing.Private = updated.Private;
        existing.Fork = updated.Fork;
        existing.Homepage = updated.Homepage;
        existing.Language = updated.Language;
        existing.ForksCount = updated.ForksCount;
        existing.StargazersCount = updated.StargazersCount;
        existing.WatchersCount = updated.WatchersCount;
        existing.Size = updated.Size;
        existing.DefaultBranch = updated.DefaultBranch;
        existing.OpenIssuesCount = updated.OpenIssuesCount;
        existing.HasIssues = updated.HasIssues;
        existing.HasProjects = updated.HasProjects;
        existing.HasWiki = updated.HasWiki;
        existing.HasPages = updated.HasPages;
        existing.HasDownloads = updated.HasDownloads;
        existing.Archived = updated.Archived;
        existing.Disabled = updated.Disabled;
        existing.HtmlUrl = updated.HtmlUrl;
        existing.CloneUrl = updated.CloneUrl;
        existing.SshUrl = updated.SshUrl;
        existing.GitHubUpdatedAt = updated.GitHubUpdatedAt;
        existing.GitHubPushedAt = updated.GitHubPushedAt;
        existing.OrganisationId = orgId;
        existing.UpdatedAt = DateTime.UtcNow;
    }

    private static void UpdateWorkflow(GitHubWorkflow existing, GitHubWorkflow updated)
    {
        existing.Name = updated.Name;
        existing.Path = updated.Path;
        existing.State = updated.State;
        existing.HtmlUrl = updated.HtmlUrl;
        existing.BadgeUrl = updated.BadgeUrl;
        existing.GitHubUpdatedAt = updated.GitHubUpdatedAt;
        existing.UpdatedAt = DateTime.UtcNow;
    }

    private static void UpdateWorkflowRun(GitHubWorkflowRun existing, GitHubWorkflowRun updated)
    {
        existing.Name = updated.Name;
        existing.DisplayTitle = updated.DisplayTitle;
        existing.RunNumber = updated.RunNumber;
        existing.RunAttempt = updated.RunAttempt;
        existing.Event = updated.Event;
        existing.Status = updated.Status;
        existing.Conclusion = updated.Conclusion;
        existing.HeadBranch = updated.HeadBranch;
        existing.HeadSha = updated.HeadSha;
        existing.HtmlUrl = updated.HtmlUrl;
        existing.JobsUrl = updated.JobsUrl;
        existing.LogsUrl = updated.LogsUrl;
        existing.CheckSuiteUrl = updated.CheckSuiteUrl;
        existing.ArtifactsUrl = updated.ArtifactsUrl;
        existing.CancelUrl = updated.CancelUrl;
        existing.RerunUrl = updated.RerunUrl;
        existing.GitHubUpdatedAt = updated.GitHubUpdatedAt;
        existing.RunStartedAt = updated.RunStartedAt;
        existing.UpdatedAt = DateTime.UtcNow;
    }

    private static void UpdateWorkflowJob(GitHubWorkflowJob existing, GitHubWorkflowJob updated)
    {
        existing.Name = updated.Name;
        existing.Status = updated.Status;
        existing.Conclusion = updated.Conclusion;
        existing.HeadSha = updated.HeadSha;
        existing.HtmlUrl = updated.HtmlUrl;
        existing.RunnerName = updated.RunnerName;
        existing.RunnerGroupName = updated.RunnerGroupName;
        existing.CheckRunId = updated.CheckRunId;
        existing.GitHubStartedAt = updated.GitHubStartedAt;
        existing.GitHubCompletedAt = updated.GitHubCompletedAt;
        existing.UpdatedAt = DateTime.UtcNow;
    }
}
