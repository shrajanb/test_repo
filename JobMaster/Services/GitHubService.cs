using System.Text.Json;
using JobMaster.Models.GitHub;

namespace JobMaster.Services;

public class GitHubService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubService> _logger;
    private readonly string _baseUrl = "https://api.github.com";

    public GitHubService(HttpClient httpClient, ILogger<GitHubService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Set up GitHub API headers
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "JobMaster/1.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    }

    public async Task<IEnumerable<GitHubOrganisation>> GetOrganisationsAsync(string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/user/orgs");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var orgs = JsonSerializer.Deserialize<GitHubOrganisationDto[]>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });

            return orgs?.Select(MapToOrganisation) ?? Enumerable.Empty<GitHubOrganisation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub organisations");
            return Enumerable.Empty<GitHubOrganisation>();
        }
    }

    public async Task<IEnumerable<GitHubRepository>> GetRepositoriesAsync(string org, string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/orgs/{org}/repos");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<GitHubRepositoryDto[]>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });

            return repos?.Select(MapToRepository) ?? Enumerable.Empty<GitHubRepository>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub repositories for org: {Org}", org);
            return Enumerable.Empty<GitHubRepository>();
        }
    }

    public async Task<IEnumerable<GitHubWorkflow>> GetWorkflowsAsync(string owner, string repo, string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/repos/{owner}/{repo}/actions/workflows");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var workflowResponse = JsonSerializer.Deserialize<GitHubWorkflowResponseDto>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });

            return workflowResponse?.Workflows?.Select(MapToWorkflow) ?? Enumerable.Empty<GitHubWorkflow>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub workflows for {Owner}/{Repo}", owner, repo);
            return Enumerable.Empty<GitHubWorkflow>();
        }
    }

    public async Task<IEnumerable<GitHubWorkflowRun>> GetWorkflowRunsAsync(string owner, string repo, long workflowId, string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var runResponse = JsonSerializer.Deserialize<GitHubWorkflowRunResponseDto>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });

            return runResponse?.WorkflowRuns?.Select(MapToWorkflowRun) ?? Enumerable.Empty<GitHubWorkflowRun>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub workflow runs for {Owner}/{Repo}/{WorkflowId}", owner, repo, workflowId);
            return Enumerable.Empty<GitHubWorkflowRun>();
        }
    }

    public async Task<IEnumerable<GitHubWorkflowJob>> GetWorkflowJobsAsync(string owner, string repo, long runId, string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/repos/{owner}/{repo}/actions/runs/{runId}/jobs");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var jobResponse = JsonSerializer.Deserialize<GitHubWorkflowJobResponseDto>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            });

            return jobResponse?.Jobs?.Select(MapToWorkflowJob) ?? Enumerable.Empty<GitHubWorkflowJob>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub workflow jobs for {Owner}/{Repo}/{RunId}", owner, repo, runId);
            return Enumerable.Empty<GitHubWorkflowJob>();
        }
    }

    private static GitHubOrganisation MapToOrganisation(GitHubOrganisationDto dto) => new()
    {
        GitHubId = dto.Id,
        Login = dto.Login,
        Name = dto.Name,
        Description = dto.Description,
        Company = dto.Company,
        Blog = dto.Blog,
        Location = dto.Location,
        Email = dto.Email,
        TwitterUsername = dto.TwitterUsername,
        PublicRepos = dto.PublicRepos,
        PublicGists = dto.PublicGists,
        Followers = dto.Followers,
        Following = dto.Following,
        HtmlUrl = dto.HtmlUrl,
        AvatarUrl = dto.AvatarUrl,
        GitHubCreatedAt = dto.CreatedAt,
        GitHubUpdatedAt = dto.UpdatedAt
    };

    private static GitHubRepository MapToRepository(GitHubRepositoryDto dto) => new()
    {
        GitHubId = dto.Id,
        Name = dto.Name,
        FullName = dto.FullName,
        Description = dto.Description,
        Private = dto.Private,
        Fork = dto.Fork,
        Homepage = dto.Homepage,
        Language = dto.Language,
        ForksCount = dto.ForksCount,
        StargazersCount = dto.StargazersCount,
        WatchersCount = dto.WatchersCount,
        Size = dto.Size,
        DefaultBranch = dto.DefaultBranch,
        OpenIssuesCount = dto.OpenIssuesCount,
        HasIssues = dto.HasIssues,
        HasProjects = dto.HasProjects,
        HasWiki = dto.HasWiki,
        HasPages = dto.HasPages,
        HasDownloads = dto.HasDownloads,
        Archived = dto.Archived,
        Disabled = dto.Disabled,
        HtmlUrl = dto.HtmlUrl,
        CloneUrl = dto.CloneUrl,
        SshUrl = dto.SshUrl,
        GitHubCreatedAt = dto.CreatedAt,
        GitHubUpdatedAt = dto.UpdatedAt,
        GitHubPushedAt = dto.PushedAt
    };

    private static GitHubWorkflow MapToWorkflow(GitHubWorkflowDto dto) => new()
    {
        GitHubId = dto.Id,
        Name = dto.Name,
        Path = dto.Path,
        State = dto.State,
        HtmlUrl = dto.HtmlUrl,
        BadgeUrl = dto.BadgeUrl,
        GitHubCreatedAt = dto.CreatedAt,
        GitHubUpdatedAt = dto.UpdatedAt
    };

    private static GitHubWorkflowRun MapToWorkflowRun(GitHubWorkflowRunDto dto) => new()
    {
        GitHubId = dto.Id,
        Name = dto.Name,
        DisplayTitle = dto.DisplayTitle,
        RunNumber = dto.RunNumber,
        RunAttempt = dto.RunAttempt,
        Event = dto.Event,
        Status = dto.Status,
        Conclusion = dto.Conclusion,
        HeadBranch = dto.HeadBranch,
        HeadSha = dto.HeadSha,
        HtmlUrl = dto.HtmlUrl,
        JobsUrl = dto.JobsUrl,
        LogsUrl = dto.LogsUrl,
        CheckSuiteUrl = dto.CheckSuiteUrl,
        ArtifactsUrl = dto.ArtifactsUrl,
        CancelUrl = dto.CancelUrl,
        RerunUrl = dto.RerunUrl,
        GitHubCreatedAt = dto.CreatedAt,
        GitHubUpdatedAt = dto.UpdatedAt,
        RunStartedAt = dto.RunStartedAt
    };

    private static GitHubWorkflowJob MapToWorkflowJob(GitHubWorkflowJobDto dto) => new()
    {
        GitHubId = dto.Id,
        Name = dto.Name,
        Status = dto.Status,
        Conclusion = dto.Conclusion,
        HeadSha = dto.HeadSha,
        HtmlUrl = dto.HtmlUrl,
        RunnerName = dto.RunnerName,
        RunnerGroupName = dto.RunnerGroupName,
        CheckRunId = dto.CheckRunId,
        GitHubCreatedAt = dto.CreatedAt,
        GitHubStartedAt = dto.StartedAt,
        GitHubCompletedAt = dto.CompletedAt
    };
}
