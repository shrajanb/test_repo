using System.Text;
using System.Text.Json;
using JobMaster.Models.Jira;

namespace JobMaster.Services;

public class JiraService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JiraService> _logger;
    private readonly string _baseUrl;

    public JiraService(HttpClient httpClient, ILogger<JiraService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["Jira:BaseUrl"] ?? throw new InvalidOperationException("Jira:BaseUrl is required");
        
        // Set up Jira API headers
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "JobMaster/1.0");
    }

    public async Task<IEnumerable<JiraIssue>> GetIssuesAsync(string? username = null, string? token = null, string? jql = null, int maxResults = 100)
    {
        try
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
            {
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{token}"));
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
            }

            // Default JQL to get issues from last 30 days if none provided
            var query = jql ?? "updated >= -30d ORDER BY updated DESC";
            var encodedQuery = Uri.EscapeDataString(query);
            
            var allIssues = new List<JiraIssue>();
            var startAt = 0;
            
            do
            {
                var url = $"{_baseUrl}/rest/api/2/search?jql={encodedQuery}&startAt={startAt}&maxResults={maxResults}&expand=names";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch Jira issues. Status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                    break;
                }
                
                var content = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonSerializer.Deserialize<JiraSearchResponseDto>(content, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (searchResponse?.Issues == null || searchResponse.Issues.Length == 0)
                    break;

                var issues = searchResponse.Issues.Select(MapToJiraIssue);
                allIssues.AddRange(issues);
                
                startAt += maxResults;
                
                // Break if we've retrieved all issues
                if (startAt >= searchResponse.Total)
                    break;
                    
            } while (true);

            return allIssues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Jira issues");
            return Enumerable.Empty<JiraIssue>();
        }
    }

    public async Task<JiraIssue?> GetIssueByKeyAsync(string issueKey, string? username = null, string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
            {
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{token}"));
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
            }

            var url = $"{_baseUrl}/rest/api/2/issue/{issueKey}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch Jira issue {IssueKey}. Status: {StatusCode}", issueKey, response.StatusCode);
                return null;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var issueDto = JsonSerializer.Deserialize<JiraIssueDto>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            return issueDto != null ? MapToJiraIssue(issueDto) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Jira issue {IssueKey}", issueKey);
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetProjectKeysAsync(string? username = null, string? token = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
            {
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{token}"));
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/rest/api/2/project");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var projects = JsonSerializer.Deserialize<JiraProjectDto[]>(content, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            return projects?.Select(p => p.Key) ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Jira projects");
            return Enumerable.Empty<string>();
        }
    }

    private static JiraIssue MapToJiraIssue(JiraIssueDto dto)
    {
        return new JiraIssue
        {
            IssueKey = dto.Key,
            IssueId = dto.Id,
            Summary = dto.Fields.Summary,
            Description = dto.Fields.Description,
            IssueType = dto.Fields.IssueType?.Name,
            Status = dto.Fields.Status?.Name,
            Priority = dto.Fields.Priority?.Name,
            Project = dto.Fields.Project?.Name,
            ProjectKey = dto.Fields.Project?.Key,
            Reporter = dto.Fields.Reporter?.DisplayName ?? dto.Fields.Reporter?.EmailAddress,
            Assignee = dto.Fields.Assignee?.DisplayName ?? dto.Fields.Assignee?.EmailAddress,
            Creator = dto.Fields.Creator?.DisplayName ?? dto.Fields.Creator?.EmailAddress,
            Resolution = dto.Fields.Resolution?.Name,
            ResolutionDate = dto.Fields.ResolutionDate,
            DueDate = dto.Fields.DueDate,
            Labels = dto.Fields.Labels != null ? JsonSerializer.Serialize(dto.Fields.Labels) : null,
            Components = dto.Fields.Components != null ? JsonSerializer.Serialize(dto.Fields.Components.Select(c => c.Name)) : null,
            FixVersions = dto.Fields.FixVersions != null ? JsonSerializer.Serialize(dto.Fields.FixVersions.Select(v => v.Name)) : null,
            AffectedVersions = dto.Fields.AffectedVersions != null ? JsonSerializer.Serialize(dto.Fields.AffectedVersions.Select(v => v.Name)) : null,
            StoryPoints = dto.Fields.StoryPoints.HasValue ? (int?)Math.Round(dto.Fields.StoryPoints.Value) : null,
            TimeOriginalEstimate = dto.Fields.TimeOriginalEstimate,
            TimeRemaining = dto.Fields.TimeRemaining,
            TimeSpent = dto.Fields.TimeSpent,
            Epic = dto.Fields.Epic,
            Sprint = dto.Fields.Sprint != null && dto.Fields.Sprint.Length > 0 ? dto.Fields.Sprint.LastOrDefault()?.Name : null,
            IssueUrl = dto.Self,
            JiraCreatedAt = dto.Fields.CreatedAt,
            JiraUpdatedAt = dto.Fields.UpdatedAt
        };
    }
}
