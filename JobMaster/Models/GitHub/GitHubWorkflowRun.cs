using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobMaster.Models.GitHub;

[Table("github_workflow_runs")]
public class GitHubWorkflowRun
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public long GitHubId { get; set; }
    
    [MaxLength(255)]
    public string? Name { get; set; }
    
    [MaxLength(255)]
    public string? DisplayTitle { get; set; }
    
    public int RunNumber { get; set; }
    
    public int RunAttempt { get; set; }
    
    [MaxLength(50)]
    public string Event { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Conclusion { get; set; }
    
    [MaxLength(255)]
    public string? HeadBranch { get; set; }
    
    [MaxLength(255)]
    public string? HeadSha { get; set; }
    
    [MaxLength(500)]
    public string? HtmlUrl { get; set; }
    
    [MaxLength(500)]
    public string? JobsUrl { get; set; }
    
    [MaxLength(500)]
    public string? LogsUrl { get; set; }
    
    [MaxLength(500)]
    public string? CheckSuiteUrl { get; set; }
    
    [MaxLength(500)]
    public string? ArtifactsUrl { get; set; }
    
    [MaxLength(500)]
    public string? CancelUrl { get; set; }
    
    [MaxLength(500)]
    public string? RerunUrl { get; set; }
    
    public DateTime? GitHubCreatedAt { get; set; }
    
    public DateTime? GitHubUpdatedAt { get; set; }
    
    public DateTime? RunStartedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Key
    [Required]
    [ForeignKey(nameof(Workflow))]
    public int WorkflowId { get; set; }
    
    // Navigation properties
    public virtual GitHubWorkflow Workflow { get; set; } = null!;
    
    public virtual ICollection<GitHubWorkflowJob> WorkflowJobs { get; set; } = new List<GitHubWorkflowJob>();
}
