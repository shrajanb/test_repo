using Hangfire;
using JobMaster.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace JobMaster.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager,
        ILogger<JobsController> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _logger = logger;
    }

    [HttpPost("github/sync")]
    public IActionResult TriggerGitHubSync()
    {
        try
        {
            var jobId = _backgroundJobClient.Enqueue<GitHubSyncJob>(job => job.SyncAsync());
            _logger.LogInformation("GitHub sync job triggered with ID: {JobId}", jobId);
            
            return Ok(new { JobId = jobId, Message = "GitHub sync job triggered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger GitHub sync job");
            return StatusCode(500, new { Error = "Failed to trigger GitHub sync job" });
        }
    }

    [HttpPost("github/schedule")]
    public IActionResult ScheduleGitHubSync()
    {
        try
        {
            // Schedule to run daily at 2 AM
            _recurringJobManager.AddOrUpdate<GitHubSyncJob>(
                "github-sync",
                job => job.SyncAsync(),
                Cron.Daily(2)
            );
            
            _logger.LogInformation("GitHub sync job scheduled to run daily at 2 AM");
            
            return Ok(new { Message = "GitHub sync job scheduled successfully to run daily at 2 AM" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule GitHub sync job");
            return StatusCode(500, new { Error = "Failed to schedule GitHub sync job" });
        }
    }

    [HttpDelete("github/unschedule")]
    public IActionResult UnscheduleGitHubSync()
    {
        try
        {
            _recurringJobManager.RemoveIfExists("github-sync");
            _logger.LogInformation("GitHub sync job unscheduled");
            
            return Ok(new { Message = "GitHub sync job unscheduled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unschedule GitHub sync job");
            return StatusCode(500, new { Error = "Failed to unschedule GitHub sync job" });
        }
    }

    [HttpPost("jira/sync")]
    public IActionResult TriggerJiraSync()
    {
        try
        {
            var jobId = _backgroundJobClient.Enqueue<JiraSyncJob>(job => job.SyncAsync());
            _logger.LogInformation("Jira sync job triggered with ID: {JobId}", jobId);
            
            return Ok(new { JobId = jobId, Message = "Jira sync job triggered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger Jira sync job");
            return StatusCode(500, new { Error = "Failed to trigger Jira sync job" });
        }
    }

    [HttpPost("jira/schedule")]
    public IActionResult ScheduleJiraSync()
    {
        try
        {
            // Schedule to run daily at 3 AM
            _recurringJobManager.AddOrUpdate<JiraSyncJob>(
                "jira-sync",
                job => job.SyncAsync(),
                Cron.Daily(3)
            );
            
            _logger.LogInformation("Jira sync job scheduled to run daily at 3 AM");
            
            return Ok(new { Message = "Jira sync job scheduled successfully to run daily at 3 AM" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule Jira sync job");
            return StatusCode(500, new { Error = "Failed to schedule Jira sync job" });
        }
    }

    [HttpDelete("jira/unschedule")]
    public IActionResult UnscheduleJiraSync()
    {
        try
        {
            _recurringJobManager.RemoveIfExists("jira-sync");
            _logger.LogInformation("Jira sync job unscheduled");
            
            return Ok(new { Message = "Jira sync job unscheduled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unschedule Jira sync job");
            return StatusCode(500, new { Error = "Failed to unschedule Jira sync job" });
        }
    }

    [HttpGet("status")]
    public IActionResult GetJobStatus()
    {
        return Ok(new 
        { 
            Message = "Job Master API is running",
            Jobs = new[]
            {
                new { Name = "sync_github", Status = "Available" },
                new { Name = "sync_jira", Status = "Available" },
                new { Name = "sync_bitbucket", Status = "Planned" },
                new { Name = "sync_jenkins", Status = "Planned" },
                new { Name = "sync_metrics", Status = "Planned" }
            }
        });
    }
}
