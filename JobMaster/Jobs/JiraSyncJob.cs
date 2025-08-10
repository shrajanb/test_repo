using Hangfire;
using JobMaster.Data;
using JobMaster.Models.Jira;
using JobMaster.Services;
using Microsoft.EntityFrameworkCore;

namespace JobMaster.Jobs;

public class JiraSyncJob
{
    private readonly JobMasterDbContext _context;
    private readonly JiraService _jiraService;
    private readonly ILogger<JiraSyncJob> _logger;
    private readonly IConfiguration _configuration;

    public JiraSyncJob(
        JobMasterDbContext context,
        JiraService jiraService,
        ILogger<JiraSyncJob> logger,
        IConfiguration configuration)
    {
        _context = context;
        _jiraService = jiraService;
        _logger = logger;
        _configuration = configuration;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SyncAsync()
    {
        var username = _configuration["Jira:Username"];
        var token = _configuration["Jira:Token"];
        var customJql = _configuration["Jira:CustomJql"];
        
        _logger.LogInformation("Starting Jira sync job at {Time}", DateTime.UtcNow);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Jira username or token not configured. Skipping Jira sync.");
            return;
        }

        try
        {
            // Sync issues
            await SyncIssuesAsync(username, token, customJql);
            
            _logger.LogInformation("Jira sync job completed successfully at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Jira sync job failed");
            throw;
        }
    }

    private async Task SyncIssuesAsync(string username, string token, string? customJql = null)
    {
        _logger.LogInformation("Syncing Jira issues");
        
        // Get all projects first to determine scope
        var projectKeys = await _jiraService.GetProjectKeysAsync(username, token);
        _logger.LogInformation("Found {ProjectCount} Jira projects: {Projects}", 
            projectKeys.Count(), string.Join(", ", projectKeys));

        // Use custom JQL if provided, otherwise sync recent issues from all projects
        var jql = customJql;
        if (string.IsNullOrEmpty(jql))
        {
            if (projectKeys.Any())
            {
                var projectFilter = string.Join(",", projectKeys);
                jql = $"project IN ({projectFilter}) AND updated >= -30d ORDER BY updated DESC";
            }
            else
            {
                jql = "updated >= -30d ORDER BY updated DESC";
            }
        }

        _logger.LogInformation("Using JQL query: {Jql}", jql);
        
        var issues = await _jiraService.GetIssuesAsync(username, token, jql, maxResults: 100);
        
        var syncedCount = 0;
        var updatedCount = 0;
        var newCount = 0;

        foreach (var issueDto in issues)
        {
            try
            {
                // Check if issue already exists
                var existingIssue = await _context.JiraIssues
                    .FirstOrDefaultAsync(i => i.IssueKey == issueDto.IssueKey);

                if (existingIssue != null)
                {
                    // Update existing issue
                    UpdateIssue(existingIssue, issueDto);
                    updatedCount++;
                }
                else
                {
                    // Add new issue
                    _context.JiraIssues.Add(issueDto);
                    newCount++;
                }

                syncedCount++;

                // Save in batches to avoid memory issues
                if (syncedCount % 50 == 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Synced {Count} issues so far...", syncedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Jira issue {IssueKey}", issueDto.IssueKey);
            }
        }

        // Save remaining changes
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Jira sync completed. Total: {Total}, New: {New}, Updated: {Updated}", 
            syncedCount, newCount, updatedCount);
    }

    private static void UpdateIssue(JiraIssue existing, JiraIssue updated)
    {
        existing.IssueId = updated.IssueId;
        existing.Summary = updated.Summary;
        existing.Description = updated.Description;
        existing.IssueType = updated.IssueType;
        existing.Status = updated.Status;
        existing.Priority = updated.Priority;
        existing.Project = updated.Project;
        existing.ProjectKey = updated.ProjectKey;
        existing.Reporter = updated.Reporter;
        existing.Assignee = updated.Assignee;
        existing.Creator = updated.Creator;
        existing.Resolution = updated.Resolution;
        existing.ResolutionDate = updated.ResolutionDate;
        existing.DueDate = updated.DueDate;
        existing.Labels = updated.Labels;
        existing.Components = updated.Components;
        existing.FixVersions = updated.FixVersions;
        existing.AffectedVersions = updated.AffectedVersions;
        existing.StoryPoints = updated.StoryPoints;
        existing.TimeOriginalEstimate = updated.TimeOriginalEstimate;
        existing.TimeRemaining = updated.TimeRemaining;
        existing.TimeSpent = updated.TimeSpent;
        existing.Epic = updated.Epic;
        existing.Sprint = updated.Sprint;
        existing.IssueUrl = updated.IssueUrl;
        existing.JiraCreatedAt = updated.JiraCreatedAt;
        existing.JiraUpdatedAt = updated.JiraUpdatedAt;
        existing.UpdatedAt = DateTime.UtcNow;
    }
}
