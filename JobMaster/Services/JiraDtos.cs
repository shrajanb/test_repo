using System.Text.Json.Serialization;

namespace JobMaster.Services;

// Jira Issue DTO
public class JiraIssueDto
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Self { get; set; } = string.Empty;
    public JiraIssueFieldsDto Fields { get; set; } = new();
}

public class JiraIssueFieldsDto
{
    public string? Summary { get; set; }
    public string? Description { get; set; }
    
    [JsonPropertyName("issuetype")]
    public JiraIssueTypeDto? IssueType { get; set; }
    
    public JiraStatusDto? Status { get; set; }
    public JiraPriorityDto? Priority { get; set; }
    public JiraProjectDto? Project { get; set; }
    public JiraUserDto? Reporter { get; set; }
    public JiraUserDto? Assignee { get; set; }
    public JiraUserDto? Creator { get; set; }
    public JiraResolutionDto? Resolution { get; set; }
    
    [JsonPropertyName("resolutiondate")]
    public DateTime? ResolutionDate { get; set; }
    
    [JsonPropertyName("duedate")]
    public DateTime? DueDate { get; set; }
    
    public string[]? Labels { get; set; }
    public JiraComponentDto[]? Components { get; set; }
    
    [JsonPropertyName("fixVersions")]
    public JiraVersionDto[]? FixVersions { get; set; }
    
    [JsonPropertyName("versions")]
    public JiraVersionDto[]? AffectedVersions { get; set; }
    
    [JsonPropertyName("customfield_10016")] // Story Points (may vary by Jira instance)
    public double? StoryPoints { get; set; }
    
    [JsonPropertyName("timeoriginalestimate")]
    public int? TimeOriginalEstimate { get; set; }
    
    [JsonPropertyName("timeestimate")]
    public int? TimeRemaining { get; set; }
    
    [JsonPropertyName("timespent")]
    public int? TimeSpent { get; set; }
    
    [JsonPropertyName("customfield_10014")] // Epic Link (may vary by Jira instance)
    public string? Epic { get; set; }
    
    [JsonPropertyName("customfield_10020")] // Sprint (may vary by Jira instance)
    public JiraSprintDto[]? Sprint { get; set; }
    
    [JsonPropertyName("created")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated")]
    public DateTime? UpdatedAt { get; set; }
}

public class JiraIssueTypeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraStatusDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraPriorityDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraProjectDto
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraUserDto
{
    [JsonPropertyName("accountId")]
    public string? AccountId { get; set; }
    
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }
    
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }
}

public class JiraResolutionDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraComponentDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraVersionDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class JiraSprintDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

public class JiraSearchResponseDto
{
    public string Expand { get; set; } = string.Empty;
    public int StartAt { get; set; }
    public int MaxResults { get; set; }
    public int Total { get; set; }
    public JiraIssueDto[] Issues { get; set; } = Array.Empty<JiraIssueDto>();
}
