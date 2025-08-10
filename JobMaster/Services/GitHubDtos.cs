using System.Text.Json.Serialization;

namespace JobMaster.Services;

// GitHub Organisation DTO
public class GitHubOrganisationDto
{
    public long Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Company { get; set; }
    public string? Blog { get; set; }
    public string? Location { get; set; }
    public string? Email { get; set; }
    
    [JsonPropertyName("twitter_username")]
    public string? TwitterUsername { get; set; }
    
    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; set; }
    
    [JsonPropertyName("public_gists")]
    public int PublicGists { get; set; }
    
    public int Followers { get; set; }
    public int Following { get; set; }
    
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
    
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

// GitHub Repository DTO
public class GitHubRepositoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    public bool Private { get; set; }
    public bool Fork { get; set; }
    public string? Homepage { get; set; }
    public string? Language { get; set; }
    
    [JsonPropertyName("forks_count")]
    public int ForksCount { get; set; }
    
    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; set; }
    
    [JsonPropertyName("watchers_count")]
    public int WatchersCount { get; set; }
    
    public int Size { get; set; }
    
    [JsonPropertyName("default_branch")]
    public string? DefaultBranch { get; set; }
    
    [JsonPropertyName("open_issues_count")]
    public int OpenIssuesCount { get; set; }
    
    [JsonPropertyName("has_issues")]
    public bool HasIssues { get; set; }
    
    [JsonPropertyName("has_projects")]
    public bool HasProjects { get; set; }
    
    [JsonPropertyName("has_wiki")]
    public bool HasWiki { get; set; }
    
    [JsonPropertyName("has_pages")]
    public bool HasPages { get; set; }
    
    [JsonPropertyName("has_downloads")]
    public bool HasDownloads { get; set; }
    
    public bool Archived { get; set; }
    public bool Disabled { get; set; }
    
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
    
    [JsonPropertyName("clone_url")]
    public string? CloneUrl { get; set; }
    
    [JsonPropertyName("ssh_url")]
    public string? SshUrl { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonPropertyName("pushed_at")]
    public DateTime? PushedAt { get; set; }
}

// GitHub Workflow DTO
public class GitHubWorkflowDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
    
    [JsonPropertyName("badge_url")]
    public string? BadgeUrl { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

public class GitHubWorkflowResponseDto
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
    
    public GitHubWorkflowDto[]? Workflows { get; set; }
}

// GitHub Workflow Run DTO
public class GitHubWorkflowRunDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    
    [JsonPropertyName("display_title")]
    public string? DisplayTitle { get; set; }
    
    [JsonPropertyName("run_number")]
    public int RunNumber { get; set; }
    
    [JsonPropertyName("run_attempt")]
    public int RunAttempt { get; set; }
    
    public string Event { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Conclusion { get; set; }
    
    [JsonPropertyName("head_branch")]
    public string? HeadBranch { get; set; }
    
    [JsonPropertyName("head_sha")]
    public string? HeadSha { get; set; }
    
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
    
    [JsonPropertyName("jobs_url")]
    public string? JobsUrl { get; set; }
    
    [JsonPropertyName("logs_url")]
    public string? LogsUrl { get; set; }
    
    [JsonPropertyName("check_suite_url")]
    public string? CheckSuiteUrl { get; set; }
    
    [JsonPropertyName("artifacts_url")]
    public string? ArtifactsUrl { get; set; }
    
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; set; }
    
    [JsonPropertyName("rerun_url")]
    public string? RerunUrl { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonPropertyName("run_started_at")]
    public DateTime? RunStartedAt { get; set; }
}

public class GitHubWorkflowRunResponseDto
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
    
    [JsonPropertyName("workflow_runs")]
    public GitHubWorkflowRunDto[]? WorkflowRuns { get; set; }
}

// GitHub Workflow Job DTO
public class GitHubWorkflowJobDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Conclusion { get; set; }
    
    [JsonPropertyName("head_sha")]
    public string? HeadSha { get; set; }
    
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
    
    [JsonPropertyName("runner_name")]
    public string? RunnerName { get; set; }
    
    [JsonPropertyName("runner_group_name")]
    public string? RunnerGroupName { get; set; }
    
    [JsonPropertyName("check_run_id")]
    public long? CheckRunId { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("started_at")]
    public DateTime? StartedAt { get; set; }
    
    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }
}

public class GitHubWorkflowJobResponseDto
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
    
    public GitHubWorkflowJobDto[]? Jobs { get; set; }
}
